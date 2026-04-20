using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using WebApiAdaptor.Models;

namespace WebApiAdaptor.Controllers
{
    /// <summary>
    /// Provides a single endpoint that supports server-side searching, filtering, sorting, and paging
    /// for integration with Syncfusion® Blazor DataGrid configured with WebApiAdaptor.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public sealed class GridController : ControllerBase
    {
        /// <summary>
        /// Returns order data with server-side data operations applied based on OData-style query parameters.
        /// Supports:
        /// <list type="bullet">
        ///   <item>
        ///     <description><c>$filter</c> — substring search via <c>substringof</c> and simple equality/starts-with filtering.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>$orderby</c> — single or multiple field sorting with optional <c>asc</c>/<c>desc</c>.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>$skip</c>, <c>$top</c> — paging.</description>
        ///   </item>
        /// </list>
        /// </summary>
        /// <param name="cancellationToken">Propagates a notification that operations should be canceled.</param>
        /// <returns>
        /// HTTP 200 (OK) with:
        /// <list type="bullet">
        ///   <item><description><c>Items</c> — processed collection for the current request.</description></item>
        ///   <item><description><c>Count</c> — total record count prior to paging.</description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            // 1) Load base collection
            List<OrderDetails> data = await Task.FromResult(OrderDetails.GetAllRecords().ToList());

            // Compute the total record count after applying search, filter, and sort,
            // but before applying paging. This value is required for WebApiAdaptor pagination.

            // 2) Search / Filter via $filter
            string? filterQuery = Request.Query["$filter"];

            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                // Split multiple conditions joined by "and"
                string[] conditions = filterQuery.Split(
                    " and ",
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (string condition in conditions)
                {
                    // Handle substring search: substringof('value', FieldName)
                    if (condition.Contains("substringof", StringComparison.OrdinalIgnoreCase))
                    {
                        string searchValue = ExtractSubstringValue(condition);

                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            data = data
                                .Where(order =>
                                    (order.OrderID?.ToString()?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                    (order.CustomerID?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                    (order.ShipCity?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                    (order.ShipCountry?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false))
                                .ToList();
                        }

                        continue;
                    }

                    // Handle simple comparisons (e.g., "OrderID eq 10248") or quoted patterns
                    // Split tokens conservatively to extract field and value
                    string filterField = string.Empty;
                    string filterValue = string.Empty;

                    string[] parts = condition.Split('(', ')', '\'');

                    if (parts.Length < 6)
                    {
                        // Example without parentheses/quotes: OrderID eq 10248
                        // tokens[0] = field, tokens[1] = operator, tokens[2] = value
                        string[] tokens = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length >= 3)
                        {
                            filterField = tokens[0];
                            filterValue = tokens[2].Trim('\'');
                        }
                    }
                    else
                    {
                        // Example that includes parentheses/quoted values
                        // parts[3] -> field, parts[5] -> value
                        filterField = parts[3];
                        filterValue = parts[5];
                    }

                    if (string.IsNullOrWhiteSpace(filterField))
                        continue;

                    // Apply a minimal set of operators/semantics suited for the sample:
                    // - For OrderID: equality match on numeric string
                    // - For text fields: StartsWith to approximate "eq 'prefix'" semantics in sample code
                    switch (filterField)
                    {
                        case "OrderID":
                            data = data
                                .Where(item =>
                                    item is not null &&
                                    (item.OrderID?.ToString()?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) ?? false))
                                .ToList();
                            break;

                        case "CustomerID":
                            data = data
                                .Where(item =>
                                    item is not null &&
                                    (item.CustomerID?.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase) ?? false))
                                .ToList();
                            break;

                        case "ShipCity":
                            data = data
                                .Where(item =>
                                    item is not null &&
                                    (item.ShipCity?.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase) ?? false))
                                .ToList();
                            break;

                        case "ShipCountry":
                            data = data
                                .Where(item =>
                                    item is not null &&
                                    (item.ShipCountry?.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase) ?? false))
                                .ToList();
                            break;

                            // Extend with additional fields when required.
                    }
                }
            }

            // 3) Sort via $orderby
            string? orderByQuery = Request.Query["$orderby"];
            if (!string.IsNullOrWhiteSpace(orderByQuery))
            {
                IEnumerable<string> sortClauses = orderByQuery
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim());

                IOrderedEnumerable<OrderDetails>? ordered = null;

                foreach (string clause in sortClauses)
                {
                    // Each clause: "FieldName [asc|desc]"
                    string[] parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length == 0)
                        continue;

                    string fieldName = parts[0];
                    bool isDesc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

                    PropertyInfo? prop = typeof(OrderDetails).GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                    if (prop is null)
                        continue;

                    Func<OrderDetails, object?> keySelector = item => prop.GetValue(item, null);

                    ordered = ordered is null
                        ? (isDesc ? data.OrderByDescending(keySelector) : data.OrderBy(keySelector))
                        : (isDesc ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector));
                }

                if (ordered is not null)
                {
                    data = ordered.ToList();
                }
            }

            // Compute total after search/filter/sort, before paging (required by WebApiAdaptor)
            int totalRecordsCount = data.Count;

            // 4) Paging via $skip / $top
            string? skipQueryValue = Request.Query["$skip"];
            string? topQueryValue = Request.Query["$top"];

            _ = int.TryParse(skipQueryValue, out int skip);
            _ = int.TryParse(topQueryValue, out int top);

            if (skip < 0) skip = 0;
            if (top < 0) top = 0;

            IEnumerable<OrderDetails> items = top > 0
                ? data.Skip(skip).Take(top)
                : data;

            return Ok(new
            {
                Items = items.ToList(),
                Count = totalRecordsCount
            });
        }

        /// <summary>
        /// Extracts the search value from an OData <c>substringof</c> expression.
        /// </summary>
        /// <param name="condition">Expression such as <c>substringof('abc', CustomerID)</c>.</param>
        /// <returns>Extracted search value, or an empty string when the pattern is invalid.</returns>
        private static string ExtractSubstringValue(string condition)
        {
            int firstQuote = condition.IndexOf('\'');
            if (firstQuote < 0)
                return string.Empty;

            int secondQuote = condition.IndexOf('\'', firstQuote + 1);
            if (secondQuote <= firstQuote)
                return string.Empty;

            return condition.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
        }



        // -------------------------------------------------------------
        // GET BY ID (Required for CreatedAtAction in Insert Operation)
        // -------------------------------------------------------------

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">Represents the primary key of the entity to retrieve.</param>
        /// <param name="cancellationToken">Propagates a notification that operations should be canceled.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> with the entity when found; otherwise <see cref="NotFoundResult"/>.
        /// </returns>
        /// <response code="200">Returns the matched entity.</response>
        /// <response code="404">No entity exists with the specified identifier.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var entity = await Task.FromResult(
                OrderDetails.GetAllRecords().FirstOrDefault(o => o.OrderID == id));

            return entity is null ? NotFound() : Ok(entity);
        }


        // -------------------------------------------------------------
        // INSERT OPERATION (HTTP POST)
        // ------------------------------------------------------------

        /// <summary>
        /// Inserts a new entity into the in-memory collection.
        /// </summary>
        /// <param name="newRecord">Represents the entity to insert.</param>
        /// <param name="cancellationToken">Propagates a notification that operations should be canceled.</param>
        /// <returns>
        /// Returns <see cref="CreatedAtActionResult"/> with the created entity on success; 
        /// otherwise <see cref="BadRequestObjectResult"/> for validation failures.
        /// </returns>
        /// <remarks>
        /// This sample uses an in-memory collection for demonstration. Replace with persistent storage as required.
        /// </remarks>
        /// <response code="201">Entity created successfully; Location header contains the resource URL.</response>
        /// <response code="400">Validation failed or a duplicate key exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(
            [FromBody] OrderDetails? newRecord,
            CancellationToken cancellationToken)
        {
            if (newRecord is null)
                return BadRequest("Request body is required.");

            if (newRecord.OrderID == 0)
                return BadRequest("OrderID must be a non-zero value.");

            var store = OrderDetails.GetAllRecords();

            if (store.Any(o => o.OrderID == newRecord.OrderID))
                return BadRequest($"An entity with OrderID '{newRecord.OrderID}' already exists.");

            // Insert at the beginning to mirror original samples.
            store.Insert(0, newRecord);

            // Return 201 with a canonical resource location.
            return await Task.FromResult(
                CreatedAtAction(nameof(GetById), new { id = newRecord.OrderID }, newRecord));
        }

        // -------------------------------------------------------------
        // UPDATE OPERATION (HTTP PUT)
        // ------------------------------------------------------------

        /// <summary>
        /// Updates an existing entity in the in-memory collection.
        /// </summary>
        /// <param name="updatedRecord">Represents the entity with updated values.</param>
        /// <param name="cancellationToken">Propagates a notification that operations should be canceled.</param>
        /// <returns>
        /// Returns <see cref="NoContentResult"/> when the update succeeds; 
        /// <see cref="BadRequestObjectResult"/> when the input is invalid; 
        /// <see cref="NotFoundObjectResult"/> when no entity matches the provided key.
        /// </returns>
        /// <remarks>
        /// If editing of the primary key is not intended, omit assignments that modify <c>OrderID</c>.
        /// </remarks>
        /// <response code="204">Entity updated successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="404">No entity exists with the specified identifier.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(
            [FromBody] OrderDetails? updatedRecord,
            CancellationToken cancellationToken)
        {
            if (updatedRecord is null)
                return BadRequest("Request body is required.");

            if (updatedRecord.OrderID == 0)
                return BadRequest("OrderID must be provided for update.");

            var store = OrderDetails.GetAllRecords();

            var existing = store.FirstOrDefault(o => o.OrderID == updatedRecord.OrderID);
            if (existing is null)
                return NotFound($"No entity found with OrderID '{updatedRecord.OrderID}'.");

            // Apply updates to the supported editable fields.
            existing.CustomerID = updatedRecord.CustomerID;
            existing.ShipCity = updatedRecord.ShipCity;
            existing.ShipCountry = updatedRecord.ShipCountry;

            // Assign only when primary key modification is intended
            existing.OrderID = updatedRecord.OrderID;

            return await Task.FromResult(NoContent());
        }

        // -------------------------------------------------------------
        // DELETE OPERATION (HTTP DELETE)
        // ------------------------------------------------------------

        /// <summary>
        /// Deletes an entity by its primary key from the in-memory collection.
        /// </summary>
        /// <param name="id">Represents the identifier of the entity to delete.</param>
        /// <param name="cancellationToken">Propagates a notification that operations should be canceled.</param>
        /// <returns>
        /// Returns <see cref="NoContentResult"/> when the entity is deleted; 
        /// otherwise <see cref="NotFoundObjectResult"/> when the entity does not exist.
        /// </returns>
        /// <response code="204">Entity deleted successfully.</response>
        /// <response code="404">No entity exists with the specified identifier.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var store = OrderDetails.GetAllRecords();

            var toRemove = store.FirstOrDefault(o => o.OrderID == id);
            if (toRemove is null)
                return NotFound($"No entity found with OrderID '{id}'.");

            _ = store.Remove(toRemove);

            return await Task.FromResult(NoContent());
        }
    }
}