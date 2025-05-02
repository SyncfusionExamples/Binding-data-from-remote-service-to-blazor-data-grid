using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.Data;
using WebApiAdaptor.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiAdaptor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GridController : ControllerBase
    {
        [HttpGet]
        public object GetOrderData()
        {
            var queryString = Request.Query;
            List<OrdersDetails> data = OrdersDetails.GetAllRecords().ToList();

#nullable enable
            string? sort = queryString["$orderby"];
            string? filterQuery = queryString["$filter"];
#nullable disable

            // Sorting
            if (!string.IsNullOrEmpty(sort))
            {
                var sortConditions = sort.Split(',');
                IOrderedEnumerable<OrdersDetails>? orderedData = null;

                foreach (var sortCondition in sortConditions)
                {
                    var sortParts = sortCondition.Trim().Split(' ');
                    var sortBy = sortParts[0];
                    var descending = sortParts.Length > 1 && sortParts[1].ToLower() == "desc";

                    Func<OrdersDetails, object?> keySelector = item =>
                        item.GetType().GetProperty(sortBy)?.GetValue(item, null);

                    orderedData = orderedData == null
                        ? (descending ? data.OrderByDescending(keySelector) : data.OrderBy(keySelector))
                        : (descending ? orderedData.ThenByDescending(keySelector) : orderedData.ThenBy(keySelector));
                }

                data = orderedData?.ToList() ?? data;
            }

            // Filtering
            if (!string.IsNullOrEmpty(filterQuery))
            {
                var filterConditions = filterQuery.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var condition in filterConditions)
                {
                    if (condition.Contains("substringof"))
                    {
                        var conditionParts = condition.Split('(', ')', '\'');
                        var searchValue = conditionParts[3]?.ToLower() ?? "";

                        data = data.Where(order =>
                            order != null &&
                            (order.OrderID.ToString().Contains(searchValue) ||
                            (order.CustomerID?.ToLower().Contains(searchValue) ?? false) ||
                            (order.ShipCity?.ToLower().Contains(searchValue) ?? false) ||
                            (order.ShipCountry?.ToLower().Contains(searchValue) ?? false))
                        ).ToList();
                    }
                    else
                    {
                        string filterfield = "";
                        string filtervalue = "";
                        var filterParts = condition.Split('(', ')', '\'');

                        if (filterParts.Length < 6)
                        {
                            var filterValueParts = filterParts[1].Split();
                            filterfield = filterValueParts[0];
                            filtervalue = filterValueParts.Length > 2 ? filterValueParts[2].Trim('\'') : "";
                        }
                        else
                        {
                            filterfield = filterParts[3];
                            filtervalue = filterParts[5];
                        }

                        switch (filterfield)
                        {
                            case "OrderID":
                                data = data.Where(item => item != null && item.OrderID.ToString() == filtervalue).ToList();
                                break;
                            case "CustomerID":
                                data = data.Where(item => item != null && item.CustomerID?.ToLower().StartsWith(filtervalue.ToLower()) == true).ToList();
                                break;
                            case "ShipCity":
                                data = data.Where(item => item != null && item.ShipCity?.ToLower().StartsWith(filtervalue.ToLower()) == true).ToList();
                                break;
                            case "ShipCountry":
                                data = data.Where(item => item != null && item.ShipCountry?.ToLower().StartsWith(filtervalue.ToLower()) == true).ToList();
                                break;
                        }
                    }
                }
            }

            // Paging
            int skip = Convert.ToInt32(queryString["$skip"]);
            int take = Convert.ToInt32(queryString["$top"]);
            int totalRecordsCount = data.Count;

            return take != 0
                ? new { Items = data.Skip(skip).Take(take).ToList(), Count = totalRecordsCount }
                : new { Items = data, Count = totalRecordsCount };
        }

        // POST: api/Orders
        /// <summary>
        /// Inserts a new data item into the data collection.
        /// </summary>
        /// <param name="newRecord">It holds new record detail which is need to be inserted.</param>
        /// <returns>Returns void</returns>
        [HttpPost]
        public void Post([FromBody] OrdersDetails newRecord)
        {
            // Insert a new record into the OrdersDetails model
            OrdersDetails.GetAllRecords().Insert(0, newRecord);
        }

        // PUT: api/Grid/5
        /// <summary>
        /// Update a existing data item from the data collection.
        /// </summary>
        /// <param name="updatedOrder">It holds updated record detail which is need to be updated.</param>
        /// <returns>Returns void</returns>
        [HttpPut]
        public void Put([FromBody] OrdersDetails updatedOrder)
        {
            var id = updatedOrder.OrderID;
            // Find the existing order by ID
            var existingOrder = OrdersDetails.GetAllRecords().FirstOrDefault(o => o.OrderID == id);
            if (existingOrder != null)
            {
                // If the order exists, update its properties
                existingOrder.OrderID = updatedOrder.OrderID;
                existingOrder.CustomerID = updatedOrder.CustomerID;
                existingOrder.ShipCity = updatedOrder.ShipCity;
                existingOrder.ShipCountry = updatedOrder.ShipCountry;
            }
        }

        // DELETE api/Grid/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // Find the order to remove by ID
            var orderToRemove = OrdersDetails.GetAllRecords().FirstOrDefault(order => order.OrderID == id);
            // If the order exists, remove it
            if (orderToRemove != null)
            {
                OrdersDetails.GetAllRecords().Remove(orderToRemove);
            }
        }

        public class CRUDModel<T> where T : class
        {
#nullable enable
            public string? action { get; set; }
            public string? keyColumn { get; set; }
            public object? key { get; set; }
            public T? value { get; set; }
            public List<T>? added { get; set; }
            public List<T>? changed { get; set; }
            public List<T>? deleted { get; set; }
            public IDictionary<string, object>? @params { get; set; }
#nullable disable
        }
    }
}
