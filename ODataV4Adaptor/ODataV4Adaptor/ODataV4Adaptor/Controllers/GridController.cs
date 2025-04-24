using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ODataV4Adaptor.Client.Models;

namespace ODataV4Adaptor.Controllers
{
    /// <summary>
    /// Handles HTTP requests for grid data operations via OData.
    /// </summary>
    [ApiController]
    [Route("[controller]")]

    public class GridController : ControllerBase
    {
        /// <summary>
        /// Retrieves all order records from the data source.
        /// </summary>
        /// <remarks>
        /// This endpoint supports OData query options such as $filter, $orderby, $top, $skip, etc.,
        /// allowing clients to perform flexible queries directly from the client-side Grid.
        /// </remarks>
        /// <returns>
        /// Returns an IActionResult that contains a queryable list of ordersdetails.
        /// </returns>
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var data = OrdersDetails.GetAllRecords().AsQueryable();
            return Ok(data);
        }
        /// <summary>
        /// Inserts a new order to the collection.
        /// </summary>
        /// <param name="addRecord">The order to be inserted.</param>
        /// <returns>It returns the newly inserted record detail.</returns>
        [HttpPost]
        [EnableQuery]
        public IActionResult Post([FromBody] OrdersDetails addRecord)
        {
            if (addRecord == null)
            {
                return BadRequest("Null order");
            }
            OrdersDetails.GetAllRecords().Insert(0, addRecord);
            return new JsonResult(addRecord);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="key">The ID of the order to update.</param>
        /// <param name="updateRecord">The updated order details.</param>
        /// <returns>It returns the updated order details.</returns>
        [HttpPatch("{key}")]
        public IActionResult Patch(int key, [FromBody] OrdersDetails updateRecord)
        {
            if (updateRecord == null)
            {
                return BadRequest("No records");
            }
            var existingOrder = OrdersDetails.GetAllRecords().FirstOrDefault(order => order.OrderID == key);
            if (existingOrder != null)
            {
                // If the order exists, update its properties
                existingOrder.CustomerID = updateRecord.CustomerID ?? existingOrder.CustomerID;
                existingOrder.EmployeeID = updateRecord.EmployeeID ?? existingOrder.EmployeeID;
                existingOrder.ShipCountry = updateRecord.ShipCountry ?? existingOrder.ShipCountry;
            }
            return new JsonResult(updateRecord);
        }

        /// <summary>
        /// Deletes an order.
        /// </summary>
        /// <param name="key">The ID of the order to delete.</param>
        /// <returns>It returns the deleted record detail</returns>
        [HttpDelete("{key}")]
        public IActionResult Delete(int key)
        {
            var deleteRecord = OrdersDetails.GetAllRecords().FirstOrDefault(order => order.OrderID == key);
            if (deleteRecord != null)
            {
                OrdersDetails.GetAllRecords().Remove(deleteRecord);
            }
            return new JsonResult(deleteRecord);
        }      

    }
}
