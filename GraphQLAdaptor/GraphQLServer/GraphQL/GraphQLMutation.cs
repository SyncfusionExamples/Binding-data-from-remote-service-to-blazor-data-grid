using GraphQLServer.Models;

namespace GraphQLServer.GraphQL
{
    public class GraphQLMutation
    {
            public OrderData CreateOrder(OrderData record, int index, string action,
                [GraphQLType(typeof(AnyType))] IDictionary<string, object> additionalParameters)
            {
                var orders = OrderData.GetAllRecords();
                if (index >= 0 && index <= orders.Count)
                {
                    orders.Insert(index, record);
                }
                else
                {
                    orders.Add(record);
                }
                return record;
            }

            public OrderData UpdateOrder(OrderData record, string action, string primaryColumnName, int primaryColumnValue,
                [GraphQLType(typeof(AnyType))] IDictionary<string, object> additionalParameters)
            {
                var existingOrder = OrderData.GetAllRecords().FirstOrDefault(x => x.OrderID == primaryColumnValue);
                if (existingOrder != null)
                {
                    existingOrder.CustomerID = record.CustomerID;
                    existingOrder.EmployeeID = record.EmployeeID;
                    existingOrder.ShipCity = record.ShipCity;
                    existingOrder.ShipCountry = record.ShipCountry;
            }
                return existingOrder;
            }

            public OrderData DeleteOrder(int primaryColumnValue, string action, string primaryColumnName,
                [GraphQLType(typeof(AnyType))] IDictionary<string, object> additionalParameters)
            {
                var orders = OrderData.GetAllRecords();
                var orderToDelete = orders.FirstOrDefault(x => x.OrderID == primaryColumnValue);
                if (orderToDelete != null)
                {
                    orders.Remove(orderToDelete);
                }
                return orderToDelete;
            }

            public List<OrderData> BatchUpdate(List<OrderData>? changed, List<OrderData>? added,
                List<OrderData>? deleted, string action, string primaryColumnName,
                [GraphQLType(typeof(AnyType))] IDictionary<string, object> additionalParameters, int? dropIndex)
            {
                var orders = OrderData.GetAllRecords();

                // Update existing orders
                if (changed != null)
                {
                    foreach (var changedOrder in changed)
                    {
                        var order = orders.FirstOrDefault(e => e.OrderID == changedOrder.OrderID);
                        if (order != null)
                        {
                            order.CustomerID = changedOrder.CustomerID;
                            order.OrderDate = changedOrder.OrderDate;
                            order.Freight = changedOrder.Freight;
                        }
                    }
                }

                // Add new orders
                if (added != null)
                {
                    if (dropIndex.HasValue && dropIndex >= 0 && dropIndex <= orders.Count)
                    {
                        orders.InsertRange(dropIndex.Value, added);
                    }
                    else
                    {
                        orders.AddRange(added);
                    }
                }

                // Delete orders
                if (deleted != null)
                {
                    foreach (var deletedOrder in deleted)
                    {
                        var orderToRemove = orders.FirstOrDefault(e => e.OrderID == deletedOrder.OrderID);
                        if (orderToRemove != null)
                        {
                            orders.Remove(orderToRemove);
                        }
                    }
                }

                return orders;
            }

        }
}
