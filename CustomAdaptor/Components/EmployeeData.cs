namespace CustomAdaptor.Components.Pages
{
    public class EmployeeData
    {
        public static List<EmployeeData> Employees = new List<EmployeeData>();

        public EmployeeData() { }

        public EmployeeData(int EmployeeID, string FirstName, string LastName, string Title, string Country)
        {
            this.EmployeeID = EmployeeID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Title = Title;
            this.Country = Country;
        }

        public static List<EmployeeData> GetAllRecords()
        {
            if (Employees.Count == 0)
            {
                var firstNames = new string[] { "Alice", "John", "Claire", "Michael", "Sophia", "William", "Emma", "James", "Olivia", "Ethan" };
                var lastNames = new string[] { "Smith", "Doe", "Johnson", "Brown", "Davis", "Wilson", "Martinez", "Anderson", "Taylor", "Thomas" };
                var titles = new string[] { "Sales Representative", "Vice President, Sales", "Sales Manager", "Inside Sales Coordinator" };
                var countries = new string[] { "USA", "UK", "UAE", "NED", "BER" };

                Random random = new Random();
                for (int i = 1; i <= 100; i++)
                {
                    Employees.Add(new EmployeeData(
                        i,
                        firstNames[random.Next(firstNames.Length)],
                        lastNames[random.Next(lastNames.Length)],
                        titles[random.Next(titles.Length)],
                        countries[random.Next(countries.Length)]
                    ));
                }
            }
            return Employees;
        }
        public int EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public string? Country { get; set; }
    }
}
