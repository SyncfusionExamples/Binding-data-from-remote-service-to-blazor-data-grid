namespace CustomAdaptor.Models
{
    /// <summary>
    /// Represents an order entity used as the data contract for CustomAdaptor-based binding.
    /// </summary>
    public sealed class OrderDetails
    {
        private static readonly List<OrderDetails> _orders = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetails"/> class.
        /// </summary>
        public OrderDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetails"/> class with values.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="customerId">Customer identifier.</param>
        /// <param name="shipCity">Destination city.</param>
        /// <param name="shipCountry">Destination country.</param>
        /// <param name="freight">Freight amount.</param>
        /// <param name="orderDate">Order creation date.</param>
        public OrderDetails(
            int orderId,
            string customerId,
            string shipCity,
            string shipCountry,
            double freight,
            DateTime orderDate)
        {
            OrderId = orderId;
            CustomerId = customerId;
            ShipCity = shipCity;
            ShipCountry = shipCountry;
            Freight = freight;
            OrderDate = orderDate;
        }

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the destination city.
        /// </summary>
        public string ShipCity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the destination country.
        /// </summary>
        public string ShipCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the freight amount.
        /// </summary>
        public double Freight { get; set; }

        /// <summary>
        /// Gets or sets the order creation date.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Returns an in-memory collection of orders.
        /// </summary>
        /// <returns>A list of <see cref="OrderDetails"/> instances.</returns>
        /// <remarks>
        /// The collection is initialized once and reused across requests.
        /// </remarks>
        public static List<OrderDetails> GetAllRecords()
        {
            if (_orders.Count == 0)
            {
                Seed(_orders);
            }

            return _orders;
        }

        private static void Seed(List<OrderDetails> target)
        {
            var id = 10000;

            for (var i = 1; i <= 15; i++)
            {
                target.Add(new OrderDetails(id + 1, "ALFKI", "Berlin", "Germany", 12.5 * i, new DateTime(2025, 01, 05)));
                target.Add(new OrderDetails(id + 2, "ANATR", "Madrid", "Spain", 14.0 * i, new DateTime(2025, 02, 04)));
                target.Add(new OrderDetails(id + 3, "ANTON", "Rome", "Italy", 16.0 * i, new DateTime(2025, 03, 06)));
                target.Add(new OrderDetails(id + 4, "BLONP", "Paris", "France", 18.5 * i, new DateTime(2025, 04, 08)));
                target.Add(new OrderDetails(id + 5, "BOLID", "Lisbon", "Portugal", 21.0 * i, new DateTime(2025, 05, 10)));

                id += 5;
            }
        }
    }
}