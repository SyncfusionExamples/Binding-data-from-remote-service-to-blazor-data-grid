namespace WebApiAdaptor.Models
{
    /// <summary>
    /// Represents an order entity used in Web API samples that integrate with the
    /// Syncfusion® Blazor DataGrid via <c>WebApiAdaptor</c>.
    /// </summary>
    /// <remarks>
    /// This type is intended for demonstration purposes and uses an in-memory
    /// backing store. Replace with a persistent data store for production.
    /// </remarks>
    public sealed class OrderDetails
    {
        // In-memory backing store (shared for the app lifetime).
        private static readonly List<OrderDetails> _orders = new();

        // ✅ Static constructor seeds once before the first use of this type.
        static OrderDetails() => Seed(_orders);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetails"/> class.
        /// </summary>
        public OrderDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetails"/> class with all fields.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="customerId">Customer identifier.</param>
        /// <param name="employeeId">Employee identifier.</param>
        /// <param name="freight">Freight charge associated with the order.</param>
        /// <param name="verified">Indicates whether the order is verified.</param>
        /// <param name="orderDate">Order creation date.</param>
        /// <param name="shipCity">Destination city.</param>
        /// <param name="shipName">Recipient or shipping name.</param>
        /// <param name="shipCountry">Destination country.</param>
        /// <param name="shippedDate">Shipment date.</param>
        /// <param name="shipAddress">Destination address.</param>
        public OrderDetails(
            int orderId,
            string customerId,
            int employeeId,
            double freight,
            bool verified,
            DateTime orderDate,
            string shipCity,
            string shipName,
            string shipCountry,
            DateTime shippedDate,
            string shipAddress)
        {
            OrderID = orderId;
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

        /// <summary>
        /// Returns the in-memory collection of orders.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> containing <see cref="OrderDetails"/> entities.</returns>
        public static List<OrderDetails> GetAllRecords() => _orders;

        /// <summary>
        /// Seeds the provided collection with deterministic demo data.
        /// </summary>
        /// <param name="target">A target list to populate.</param>
        private static void Seed(List<OrderDetails> target)
        {
            var code = 10000;

            for (var i = 1; i < 10; i++)
            {
                target.Add(new OrderDetails(
                    code + 1, "ALFKI", i + 0, 2.3 * i, false,
                    new DateTime(1991, 05, 15), "Berlin", "Simons bistro", "Denmark",
                    new DateTime(1996, 07, 16), "Kirchgasse 6"));

                target.Add(new OrderDetails(
                    code + 2, "ANATR", i + 2, 3.3 * i, true,
                    new DateTime(1990, 04, 04), "Madrid", "Queen Cozinha", "Brazil",
                    new DateTime(1996, 09, 11), "Avda. Azteca 123"));

                target.Add(new OrderDetails(
                    code + 3, "ANTON", i + 1, 4.3 * i, true,
                    new DateTime(1957, 11, 30), "Cholchester", "Frankenversand", "Germany",
                    new DateTime(1996, 10, 07), "Carrera 52 con Ave. Bolívar #65-98 Llano Largo"));

                target.Add(new OrderDetails(
                    code + 4, "BLONP", i + 3, 5.3 * i, false,
                    new DateTime(1930, 10, 22), "Marseille", "Ernst Handel", "Austria",
                    new DateTime(1996, 12, 30), "Magazinweg 7"));

                target.Add(new OrderDetails(
                    code + 5, "BOLID", i + 4, 6.3 * i, true,
                    new DateTime(1953, 02, 18), "Tsawassen", "Hanari Carnes", "Switzerland",
                    new DateTime(1997, 12, 03), "1029 - 12th Ave. S."));

                code += 5;
            }
        }

        // =======================
        // Public properties
        // =======================

        /// <summary>Gets or sets the order identifier.</summary>
        public int? OrderID { get; set; }

        /// <summary>Gets or sets the customer identifier.</summary>
        public string? CustomerID { get; set; }

        /// <summary>Gets or sets the employee identifier.</summary>
        public int? EmployeeID { get; set; }

        /// <summary>Gets or sets the freight charge.</summary>
        public double? Freight { get; set; }

        /// <summary>Gets or sets the destination city.</summary>
        public string? ShipCity { get; set; }

        /// <summary>Gets or sets a value indicating whether the order is verified.</summary>
        public bool? Verified { get; set; }

        /// <summary>Gets or sets the order date.</summary>
        public DateTime OrderDate { get; set; }

        /// <summary>Gets or sets the recipient or shipping name.</summary>
        public string? ShipName { get; set; }

        /// <summary>Gets or sets the destination country.</summary>
        public string? ShipCountry { get; set; }

        /// <summary>Gets or sets the shipment date.</summary>
        public DateTime ShippedDate { get; set; }

        /// <summary>Gets or sets the destination address.</summary>
        public string? ShipAddress { get; set; }
    }
}