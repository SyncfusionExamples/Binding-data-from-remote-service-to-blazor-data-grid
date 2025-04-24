using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using URLAdaptor.Models;

namespace URLAdaptor.Controllers
{    
    [ApiController]
    public class GridController : ControllerBase
    {
        /// <summary>
        /// Retrieve data from the data source.
        /// </summary>
        /// <returns>Returns a list of ordersdetails records.</returns>
        [HttpGet]
        [Route("api/[controller]")]
        public List<OrdersDetails> GetOrderData()
        {
            return OrdersDetails.GetAllRecords().ToList();
        }

        /// <summary>
        /// Handles server-side data operations such as searching, filtering, sorting, paging, and returns the processed data.
        /// </summary>
        /// <param name="DataManagerRequest">The request object contains data operation parameters such as search, filter, sort, and pagination details.</param>
        /// <returns>Returns a response containing the processed data and the total record count.</returns>
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            // Retrieve data from the data source (e.g., database).
            IQueryable<OrdersDetails> DataSource = GetOrderData().AsQueryable();    

            // Handling searching operation.
            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
                // Add custom logic here if needed and remove above method.
            }

            // Handling filtering operation.
            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                foreach (var condition in DataManagerRequest.Where)
                {
                    foreach (var predicate in condition.predicates)
                    {
                        DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, predicate.Operator);
                        // Add custom logic here if needed and remove above method.
                    }
                }                
            }

            // Handling sorting operation.
            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
                // Add custom logic here if needed and remove above method.
            }

            // Get the total records count.
            int totalRecordsCount = DataSource.Count();

            // Handling paging operation.
            if (DataManagerRequest.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
                // Add custom logic here if needed and remove above method.
            }
            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
                // Add custom logic here if needed and remove above method.
            }

            // Return data based on the request.
            return new { result = DataSource, count = totalRecordsCount };
        }

        /// <summary>
        /// Inserts a new data item into the data collection.
        /// </summary>
        /// <param name="value">It contains the new record detail which is need to be inserted.</param>
        /// <returns>Returns void.</returns>
        [HttpPost]
        [Route("api/[controller]/Insert")]
        public void Insert([FromBody] CRUDModel<OrdersDetails> newRecord)
        {
            if (newRecord.value != null)
            {
                // Add the new record to the data collection.
                OrdersDetails.GetAllRecords().Insert(0, newRecord.value);
            }
        }

        /// <summary>
        /// Update a existing data item from the data collection.
        /// </summary>
        /// <param name="updatedRecord">It contains the updated record detail which is need to be updated.</param>
        /// <returns>Returns void.</returns>
        [HttpPost]
        [Route("api/[controller]/Update")]
        public void Update([FromBody] CRUDModel<OrdersDetails> updatedRecord)
        {
            var updatedOrder = updatedRecord.value;
            if (updatedOrder != null)
            {
                var data = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == updatedOrder.OrderID);
                if (data != null)
                {
                    // Update the existing record.
                    data.OrderID = updatedOrder.OrderID;
                    data.CustomerID = updatedOrder.CustomerID;
                    data.ShipCity = updatedOrder.ShipCity;
                    data.ShipCountry = updatedOrder.ShipCountry;
                    // Update other properties similarly.
                }
            }
        }

        /// <summary>
        /// Remove a specific data item from the data collection.
        /// </summary>
        /// <param name="deletedRecord">It contains the specific record detail which is need to be removed.</param>
        /// <return>Returns void.</return>
        [HttpPost]
        [Route("api/[controller]/Remove")]
        public void Remove([FromBody] CRUDModel<OrdersDetails> deletedRecord)
        {
            // Get the key value from the deletedRecord.
            int orderId = int.Parse(deletedRecord.key.ToString());
            var data = OrdersDetails.GetAllRecords().FirstOrDefault(orderData => orderData.OrderID == orderId);
            if (data != null)
            {
                // Remove the record from the data collection.
                OrdersDetails.GetAllRecords().Remove(data);
            }
        }

        /// <summary>
        /// Handles CRUD (Create, Read, Update, and Delete) operations for a single request using the specified CRUD URL.
        /// </summary>
        /// <param name="request">An object containing the details of the record to be processed and the action to be performed (e.g., Create, Read, Update, and Delete).</param>
        /// <returns>A response indicating the result of the CRUD operation, including success or failure details.</returns>
        [HttpPost]
        [Route("api/[controller]/CrudUpdate")]
        public void CrudUpdate([FromBody] CRUDModel<OrdersDetails> request)
        {
            // Perform the update operation.
            if (request.action == "update")
            {
                var orderValue = request.value;
                OrdersDetails existingRecord = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == orderValue.OrderID);
                if (existingRecord != null)
                {
                    // Update the properties of the existing record.
                    existingRecord.OrderID = orderValue.OrderID;
                    existingRecord.CustomerID = orderValue.CustomerID;
                    existingRecord.ShipCity = orderValue.ShipCity;
                    existingRecord.ShipCountry = orderValue.ShipCountry;
                    // Update other properties as needed.
                }
            }
            // Perform the insert operation.
            else if (request.action == "insert")
            {
                // Add the new record to the data collection.
                OrdersDetails.GetAllRecords().Insert(0, request.value);
            }
            // Perform the remove operation.
            else if (request.action == "remove")
            {
                // Remove the record from the data collection.
                OrdersDetails.GetAllRecords().Remove(OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == int.Parse(request.key.ToString())));
            }
        }

        /// <summary>
        /// Handles CRUD operations when batch editing is enabled in the DataGrid.
        /// </summary>
        /// <param name="batchModel">The batch model containing the data changes to be processed.</param>
        /// <returns>Returns the result of the CRUD operation.</returns>
        [HttpPost]
        [Route("api/[controller]/BatchUpdate")]
        public IActionResult BatchUpdate([FromBody] CRUDModel<OrdersDetails> batchModel)
        {
            // Check if there are any added records in the batch model.
            if (batchModel.added != null)
            {
                // Iterate through each added record.
                foreach (var Record in batchModel.added)
                {
                    // Insert the added record at the beginning of the existing records.
                    OrdersDetails.GetAllRecords().Insert(0, Record);
                }
            }
            // Check if there are any changed records in the batch model.
            if (batchModel.changed != null)
            {
                // Iterate through each changed record.
                foreach (var changedOrder in batchModel.changed)
                {
                    // Find the existing record that matches the changed record's OrderID.
                    var existingOrder = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == changedOrder.OrderID);
                    if (existingOrder != null)
                    {
                        // Update the properties of the existing record.
                        existingOrder.OrderID = changedOrder.OrderID;
                        existingOrder.CustomerID = changedOrder.CustomerID;
                        existingOrder.ShipCity = changedOrder.ShipCity;
                        existingOrder.ShipCountry = changedOrder.ShipCity;
                        // Update other properties as needed.
                    }
                }
            }
            // Check if there are any deleted records in the batch model.
            if (batchModel.deleted != null)
            {
                // Iterate through each deleted record.
                foreach (var deletedOrder in batchModel.deleted)
                {
                    // Find the existing record that matches the deleted record's OrderID.
                    var orderToDelete = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == deletedOrder.OrderID);
                    if (orderToDelete != null)
                    {
                        // Remove the matching record from the existing records.
                        OrdersDetails.GetAllRecords().Remove(orderToDelete);
                    }
                }
            }

            // Return the updated batch model as a JSON result.
            return new JsonResult(batchModel);
        }

        public class CRUDModel<T> where T : class
        {
            public string? action { get; set; }
            public string? keyColumn { get; set; }
            public object? key { get; set; }
            public T? value { get; set; }
            public List<T>? added { get; set; }
            public List<T>? changed { get; set; }
            public List<T>? deleted { get; set; }
            public IDictionary<string, object>? @params { get; set; }
        }
    }
}
