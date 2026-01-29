namespace Grid_GraphQLAdaptor.Models;


/// <summary>
/// Represents the input structure for data manager requests.
/// </summary>
public class DataManagerRequestInput
{
    [GraphQLName("Skip")]
    public int Skip { get; set; }

    [GraphQLName("Take")]
    public int Take { get; set; }

    [GraphQLName("RequiresCounts")]
    public bool RequiresCounts { get; set; } = false;

    [GraphQLName("Params")]
    [GraphQLType(typeof(AnyType))]
    public IDictionary<string, object> Params { get; set; }

    [GraphQLName("Aggregates")]
    [GraphQLType(typeof(AnyType))]
    public List<Aggregate>? Aggregates { get; set; }

    [GraphQLName("Search")]
    public List<SearchFilter>? Search { get; set; }

    [GraphQLName("Sorted")]
    public List<Sort>? Sorted { get; set; }

    [GraphQLName("Where")]
    [GraphQLType(typeof(AnyType))]
    public List<WhereFilter>? Where { get; set; }

    [GraphQLName("Group")]
    public List<string>? Group { get; set; }

    [GraphQLName("antiForgery")]
    public string? antiForgery { get; set; }

    [GraphQLName("Table")]
    public string? Table { get; set; }

    [GraphQLName("IdMapping")]
    public string? IdMapping { get; set; }

    [GraphQLName("Select")]
    public List<string>? Select { get; set; }

    [GraphQLName("Expand")]
    public List<string>? Expand { get; set; }

    [GraphQLName("Distinct")]
    public List<string>? Distinct { get; set; }

    [GraphQLName("ServerSideGroup")]
    public bool? ServerSideGroup { get; set; }

    [GraphQLName("LazyLoad")]
    public bool? LazyLoad { get; set; }

    [GraphQLName("LazyExpandAllGroup")]
    public bool? LazyExpandAllGroup { get; set; }
}

/// <summary>
/// Represents an aggregate operation in the data manager request.
/// </summary>
public class Aggregate
{
    [GraphQLName("Field")]
    public string Field { get; set; }

    [GraphQLName("Type")]
    public string Type { get; set; }
}

/// <summary>
/// Represents a search filter in the data manager request.
/// </summary>
public class SearchFilter
{
    [GraphQLName("Fields")]
    public List<string> Fields { get; set; }

    [GraphQLName("Key")]
    public string Key { get; set; }

    [GraphQLName("Operator")]
    public string Operator { get; set; }

    [GraphQLName("IgnoreCase")]
    public bool IgnoreCase { get; set; }

    [GraphQLName("IgnoreAccent")]
    public bool IgnoreAccent { get; set; }
}

/// <summary>
/// Represents a sorting operation in the data manager request.
/// </summary>
public class Sort
{
    [GraphQLName("Name")]
    public string Name { get; set; }

    [GraphQLName("Direction")]
    public string Direction { get; set; }

    [GraphQLName("Comparer")]
    [GraphQLType(typeof(AnyType))]
    public object Comparer { get; set; }
}

/// <summary>
/// Represents a filter condition in the data manager request.
/// </summary>
public class WhereFilter
{
    [GraphQLName("Field")]
    public string? Field { get; set; }

    [GraphQLName("IgnoreCase")]
    public bool? IgnoreCase { get; set; }

    [GraphQLName("IgnoreAccent")]
    public bool? IgnoreAccent { get; set; }

    [GraphQLName("IsComplex")]
    public bool? IsComplex { get; set; }

    [GraphQLName("Operator")]
    public string? Operator { get; set; }

    [GraphQLName("Condition")]
    public string? Condition { get; set; }

    [GraphQLName("Value")]
    [GraphQLType(typeof(AnyType))]
    public object? Value { get; set; }

    [GraphQLName("predicates")]
    public List<WhereFilter>? Predicates { get; set; }
}
