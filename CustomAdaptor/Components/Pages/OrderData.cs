namespace CustomAdaptor.Components.Pages
{
        public enum OrderStatus
        {
            Processing,
            Shipped,
            Delivered,
            Cancelled
        }

        // Complex Type for Customer Details
        public class CustomerDetails
        {
            public CustomerDetails() { } // Parameterless constructor for OData compatibility

            public CustomerDetails(string name, string email)
            {
                Name = name;
                Email = email;
            }
            public string? Name { get; set; }

            public string? Email { get; set; }
        }

        public class OrderData
        {
            public static List<OrderData> Orders = new List<OrderData>();

            public OrderData()
            {
            }

            public OrderData(
                int orderID, string customerID, int employeeID, double freight, bool verified,
                DateOnly orderDate, TimeOnly orderedTime, string shipCity, string shipName, string shipCountry,
                DateTime shippedDate, string shipAddress, OrderStatus status, CustomerDetails customerDetails)
            {
                OrderID = orderID;
                CustomerID = customerID;
                EmployeeID = employeeID;
                Freight = freight;
                Verified = verified;
                OrderDate = orderDate;
                OrderedTime = orderedTime;
                ShipCity = shipCity;
                ShipName = shipName;
                ShipCountry = shipCountry;
                ShippedDate = shippedDate;
                ShipAddress = shipAddress;
                Status = status;
                CustomerDetails = customerDetails;
            }

            public static List<OrderData> GetAllRecords()
            {
                if (Orders.Count == 0)
                {
                int code = 10000;
                int employeecode = 1;
                for (int i = 1; i <= 100; i++)
                {
                    Orders.Add(new OrderData(code + 1, "ALFKI", employeecode++, 2.3 * (i % 10 + 1), false,
                        new DateOnly(1991, 05, 15), new TimeOnly(10, i % 60, 0), "Berlin", "Simons Bistro", "Denmark",
                        new DateTime(1991, 05, 16, 10, i % 60, 0), "Kirchgasse 6", OrderStatus.Processing,
                        new CustomerDetails("John Doe", "johndoe@example.com")));

                    Orders.Add(new OrderData(code + 2, "ANATR", employeecode++, 3.3 * (i % 10 + 2), true,
                        new DateOnly(1990, 04, 04), new TimeOnly(11, i % 60, 0), "Madrid", "Queen Cozinha", "Brazil",
                        new DateTime(1990, 04, 05, 11, i % 60, 0), "Avda. Azteca 123", OrderStatus.Shipped,
                        new CustomerDetails("Jane Smith", "janesmith@example.com")));

                    Orders.Add(new OrderData(code + 3, "ANTON", employeecode++, 4.3 * (i % 10 + 3), true,
                        new DateOnly(1957, 11, 30), new TimeOnly(12, i % 60, 0), "Cholchester", "Frankenversand", "Germany",
                        new DateTime(1957, 12, 01, 12, i % 60, 0), "Carrera 52 con Ave. Bolívar #65-98 Llano Largo", OrderStatus.Delivered,
                        new CustomerDetails("Alice Johnson", "alicej@example.com")));

                    Orders.Add(new OrderData(code + 4, "BLONP", employeecode++, 5.3 * (i % 10 + 4), false,
                        new DateOnly(1930, 10, 22), new TimeOnly(13, i % 60, 0), "Marseille", "Ernst Handel", "Austria",
                        new DateTime(1930, 10, 23, 13, i % 60, 0), "Magazinweg 7", OrderStatus.Cancelled,
                        new CustomerDetails("Bob Brown", "bobbrown@example.com")));

                    Orders.Add(new OrderData(code + 5, "BERGS", employeecode++, 6.3 * (i % 10 + 5), true,
                        new DateOnly(2000, 12, 10), new TimeOnly(14, i % 60, 0), "London", "Victoria's Deli", "United Kingdom",
                        new DateTime(2000, 12, 11, 14, i % 60, 0), "221B Baker Street", OrderStatus.Processing,
                        new CustomerDetails("Charlie Green", "charliegreen@example.com")));

                    Orders.Add(new OrderData(code + 6, "CHOPS", employeecode++, 7.3 * (i % 10 + 6), false,
                        new DateOnly(2015, 06, 20), new TimeOnly(15, i % 60, 0), "Sydney", "Harbor Delight", "Australia",
                        new DateTime(2015, 06, 21, 15, i % 60, 0), "Opera House St.", OrderStatus.Shipped,
                        new CustomerDetails("Diana Prince", "dianap@example.com")));

                    Orders.Add(new OrderData(code + 7, "DUMON", employeecode++, 8.3 * (i % 10 + 7), true,
                        new DateOnly(2010, 03, 15), new TimeOnly(16, i % 60, 0), "Toronto", "Maple Leaf Bistro", "Canada",
                        new DateTime(2010, 03, 16, 16, i % 60, 0), "Maple Street 10", OrderStatus.Delivered,
                        new CustomerDetails("Edward Norton", "edwardn@example.com")));

                    Orders.Add(new OrderData(code + 8, "FOLKO", employeecode++, 9.3 * (i % 10 + 8), false,
                        new DateOnly(2020, 01, 05), new TimeOnly(17, i % 60, 0), "Paris", "Eiffel Café", "France",
                        new DateTime(2020, 01, 06, 17, i % 60, 0), "Louvre Lane", OrderStatus.Cancelled,
                        new CustomerDetails("Fiona Scott", "fiona.scott@example.com")));

                    Orders.Add(new OrderData(code + 9, "GODOS", employeecode++, 10.3 * (i % 10 + 9), true,
                        new DateOnly(2005, 07, 12), new TimeOnly(18, i % 60, 0), "Rome", "Colosseum Cuisine", "Italy",
                        new DateTime(2005, 07, 13, 18, i % 60, 0), "Via Roma 20", OrderStatus.Shipped,
                        new CustomerDetails("George Clooney", "georgec@example.com")));

                    Orders.Add(new OrderData(code + 10, "HUNGO", employeecode++, 11.3 * (i % 10 + 10), false,
                        new DateOnly(1985, 09, 18), new TimeOnly(19, i % 60, 0), "New York", "Statue Grill", "USA",
                        new DateTime(1985, 09, 19, 19, i % 60, 0), "Liberty Street", OrderStatus.Processing,
                        new CustomerDetails("Harvey Specter", "harveys@example.com")));

                    code += 10;
                }
            }
            return Orders;
            }

            public int? OrderID { get; set; }
            public string? CustomerID { get; set; }
            public int? EmployeeID { get; set; }
            public double? Freight { get; set; }
            public string? ShipCity { get; set; }
            public bool? Verified { get; set; }
            public DateOnly OrderDate { get; set; }
            public TimeOnly OrderedTime { get; set; } // Represents the order time
            public string? ShipName { get; set; }
            public string? ShipCountry { get; set; }
            public DateTime ShippedDate { get; set; } // Represents the shipped date and time
            public string? ShipAddress { get; set; }
            public OrderStatus Status { get; set; } // Enum column
            public CustomerDetails? CustomerDetails { get; set; } // Complex column
        }
}
