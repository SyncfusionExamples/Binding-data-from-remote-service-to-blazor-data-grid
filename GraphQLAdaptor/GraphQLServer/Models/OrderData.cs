using System.Text.Json.Serialization;

namespace GraphQLServer.Models
{
    public class OrderData
    {
        public static List<OrderData> Orders = new List<OrderData>();

        public OrderData() { }

        public OrderData(
            int orderID, string customerId, int employeeId, double freight, bool verified,
            DateTime orderDate, string shipCity, string shipName, string shipCountry,
            DateTime shippedDate, string shipAddress)
        {
            OrderID = orderID;
            CustomerID = customerId;
            EmployeeID = employeeId;
            Freight = freight;
            Verified = verified;
            OrderDate = orderDate;
            ShipCity = shipCity;
            ShipName = shipName;
            ShipCountry = shipCountry;
            ShippedDate = shippedDate;
            ShipAddress = shipAddress;
        }

        public static List<OrderData> GetAllRecords()
        {
            if (Orders.Count == 0)
            {
                int code = 10000;
                for (int i = 1; i < 10; i++)
                {
                    Orders.Add(new OrderData(code + 1, "ALFKI", i + 0, 2.3 * i, false, new DateTime(1991, 05, 15), "Berlin", "Simons bistro", "Denmark", new DateTime(1996, 7, 16), "Kirchgasse 6"));
                    Orders.Add(new OrderData(code + 2, "ANATR", i + 2, 3.3 * i, true, new DateTime(1990, 04, 04), "Madrid", "Queen Cozinha", "Brazil", new DateTime(1996, 9, 11), "Avda. Azteca 123"));
                    Orders.Add(new OrderData(code + 3, "ANTON", i + 1, 4.3 * i, true, new DateTime(1957, 11, 30), "Cholchester", "Frankenversand", "Germany", new DateTime(1996, 10, 7), "Carrera 52 con Ave. Bolívar #65-98 Llano Largo"));
                    Orders.Add(new OrderData(code + 4, "BLONP", i + 3, 5.3 * i, false, new DateTime(1930, 10, 22), "Marseille", "Ernst Handel", "Austria", new DateTime(1996, 12, 30), "Magazinweg 7"));
                    Orders.Add(new OrderData(code + 5, "BOLID", i + 4, 6.3 * i, true, new DateTime(1953, 02, 18), "Tsawassen", "Hanari Carnes", "Switzerland", new DateTime(1997, 12, 3), "1029 - 12th Ave. S."));
                    code += 5;
                }
            }
            return Orders;
        }

        [JsonPropertyName("orderID")]
        public int OrderID { get; set; }

        [JsonPropertyName("customerID")]
        public string? CustomerID { get; set; }

        [JsonPropertyName("employeeID")]
        public int? EmployeeID { get; set; }

        [JsonPropertyName("freight")]
        public double? Freight { get; set; }

        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }

        [JsonPropertyName("orderDate")]
        public DateTime? OrderDate { get; set; }

        [JsonPropertyName("shipCity")]
        public string ShipCity { get; set; }

        [JsonPropertyName("shipName")]
        public string? ShipName { get; set; }

        [JsonPropertyName("shipCountry")]
        public string ShipCountry { get; set; }

        [JsonPropertyName("shippedDate")]
        public DateTime? ShippedDate { get; set; }

        [JsonPropertyName("shipAddress")]
        public string? ShipAddress { get; set; }
    }
}
