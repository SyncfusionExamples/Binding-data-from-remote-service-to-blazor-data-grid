using GraphQLServer.Models;

public class GraphQLQuery
{
    public OrdersDataResponse GetOrdersData(DataManagerRequestInput dataManager)
    {
        List<OrderData> dataSource = OrderData.GetAllRecords(); // Fetch records

        // Apply search filtering
        if (dataManager.Search != null && dataManager.Search.Count > 0)
        {
            foreach (var searchFilter in dataManager.Search)
            {

                dataSource = dataSource.Where(order =>
                    searchFilter.Fields.Any(field =>
                        order.GetType().GetProperty(field)?.GetValue(order)?.ToString()
                        .IndexOf(searchFilter.Key, StringComparison.OrdinalIgnoreCase) >= 0
                    )
                ).ToList();
            }
        }

        // Apply filtering
        if (dataManager.Where != null && dataManager.Where.Count > 0)
        {
            foreach (var filter in dataManager.Where)
            {
                dataSource = dataSource.Where(order =>
                {
                    bool match = true;
                    foreach (var predicate in filter.predicates)
                    {
                        string fieldName = predicate.Field;
                        object fieldValue = predicate.Value;
                        string operation = predicate.Operator.ToLower();

                        var property = order.GetType().GetProperty(fieldName);
                        if (property == null) return false;

                        var propertyValue = property.GetValue(order);
                        if (propertyValue == null) return false;

                        switch (operation)
                        {
                            case "equal":
                                match &= propertyValue.ToString().Equals(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
                                break;
                            case "notequal":
                                match &= !propertyValue.ToString().Equals(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
                                break;
                            case "contains":
                                match &= propertyValue.ToString().IndexOf(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase) >= 0;
                                break;
                            case "startswith":
                                match &= propertyValue.ToString().StartsWith(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
                                break;
                            case "endswith":
                                match &= propertyValue.ToString().EndsWith(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
                                break;
                            case "greaterthan":
                                match &= Convert.ToDouble(propertyValue) > Convert.ToDouble(fieldValue);
                                break;
                            case "lessthan":
                                match &= Convert.ToDouble(propertyValue) < Convert.ToDouble(fieldValue);
                                break;
                            case "greaterthanequal":
                                match &= Convert.ToDouble(propertyValue) >= Convert.ToDouble(fieldValue);
                                break;
                            case "lessthanequal":
                                match &= Convert.ToDouble(propertyValue) <= Convert.ToDouble(fieldValue);
                                break;
                            default:
                                return false;
                        }
                    }
                    return match;
                }).ToList();
            }
        }

        // Apply sorting
        if (dataManager.Sorted != null && dataManager.Sorted.Count > 0)
        {
            foreach (var sort in dataManager.Sorted)
            {
                dataSource = sort.Direction.ToLower() == "ascending"
                    ? dataSource.OrderBy(order => order.GetType().GetProperty(sort.Name)?.GetValue(order)).ToList()
                    : dataSource.OrderByDescending(order => order.GetType().GetProperty(sort.Name)?.GetValue(order)).ToList();
            }
        }

        // Apply pagination
        int totalRecords = dataSource.Count;
        dataSource = dataSource.Skip(dataManager.Skip).Take(dataManager.Take).ToList();

        return new OrdersDataResponse
        {
            Count = totalRecords,
            Result = dataSource
        };
    }
}

// Response class matching GraphQL schema
public class OrdersDataResponse
{
    public int Count { get; set; }
    public List<OrderData> Result { get; set; } = new List<OrderData>();
}