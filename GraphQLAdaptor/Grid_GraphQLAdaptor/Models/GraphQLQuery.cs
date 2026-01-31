namespace Grid_GraphQLAdaptor.Models;

public class GraphQLQuery
{
    /// <summary>
    /// Retrieves all expense record and returns it along with the total record count.
    /// </summary>
    /// <param name="dataManager">The data manager request input containing query parameters.</param>
    /// <returns>An instance of <see cref="ExpenseRecordDataResponse"/> containing the expense record and count.</returns>
    public ExpenseRecordDataResponse GetExpenseRecordData(DataManagerRequestInput dataManager)
    {
        List<ExpenseRecord> dataSource = ExpenseRecord.GetAllRecords();

        // Apply search if search filters are provided.
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

        // Apply sorting if specified by the grid
        if (dataManager?.Sorted != null && dataManager.Sorted.Count > 0)
        {
            foreach (var sort in dataManager.Sorted)
            {
                var property = typeof(ExpenseRecord).GetProperty(sort.Name);
                if (property != null)
                {
                    dataSource = sort.Direction?.ToLower() == "descending"
                        ? dataSource.OrderByDescending(x => property.GetValue(x)).ToList()
                        : dataSource.OrderBy(x => property.GetValue(x)).ToList();
                }
            }
        }

        // Apply grouping if specified by the grid
        if (dataManager?.Group != null && dataManager.Group.Count > 0)
        {
            dataSource = ApplyGrouping(dataSource, dataManager.Group);
        }

        // Apply filtering if filter conditions are provided.
        if (dataManager.Where != null && dataManager.Where.Count > 0)
        {
            foreach (var filter in dataManager.Where)
            {
                dataSource = dataSource.Where(expense => EvaluateFilterGroup(expense, filter.Predicates)).ToList();
            }
        }

        int totalRecords = dataSource.Count;

        // Apply paging by skipping the specified number of records and taking the required count.
        if (dataManager.Skip != 0)
        {
            dataSource = dataSource.Skip(dataManager.Skip).ToList();
        }
        if (dataManager.Take != 0)
        {
            dataSource = dataSource.Take(dataManager.Take).ToList();
        }

        return new ExpenseRecordDataResponse
        {
            Count = totalRecords,
            Result = dataSource
        };
    }

    /// <summary>
    /// Evaluates a group of predicates against an expense record.
    /// Handles both single predicates and nested predicates (for multiple checkbox selections).
    /// Uses recursive logic to handle any depth of nesting.
    /// </summary>
    /// <param name="expense">The expense record to evaluate.</param>
    /// <param name="predicates">The list of predicates to evaluate.</param>
    /// <returns>True if the expense matches the filter conditions; otherwise, false.</returns>
    private bool EvaluateFilterGroup(ExpenseRecord expense, List<WhereFilter> predicates)
    {
        bool match = true;
        
        foreach (var predicate in predicates)
        {
            if (predicate.Predicates != null && predicate.Predicates.Count > 0)
            {
                bool nestedMatch = false;
                foreach (var nestedPredicate in predicate.Predicates)
                {
                    nestedMatch |= EvaluatePredicate(expense, nestedPredicate);
                }
                match &= nestedMatch;
            }
            else
            {
                match &= EvaluateSinglePredicate(expense, predicate);
            }
        }
        
        return match;
    }

    /// <summary>
    /// Recursively evaluates a predicate against an expense record.
    /// This method handles predicates that may contain nested predicates at any depth.
    /// </summary>
    /// <param name="expense">The expense record to evaluate.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>True if the expense matches the predicate condition; otherwise, false.</returns>
    private bool EvaluatePredicate(ExpenseRecord expense, WhereFilter predicate)
    {
        if (predicate.Predicates != null && predicate.Predicates.Count > 0)
        {
            bool nestedMatch = false;
            foreach (var nestedPredicate in predicate.Predicates)
            {
                nestedMatch |= EvaluatePredicate(expense, nestedPredicate);
            }
            return nestedMatch;
        }
        else
        {
            return EvaluateSinglePredicate(expense, predicate);
        }
    }

    /// <summary>
    /// Evaluates a single predicate against an expense record.
    /// </summary>
    /// <param name="expense">The expense record to evaluate.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>True if the expense matches the predicate condition; otherwise, false.</returns>
    private bool EvaluateSinglePredicate(ExpenseRecord expense, WhereFilter predicate)
    {
        if (string.IsNullOrEmpty(predicate.Field) || string.IsNullOrEmpty(predicate.Operator))
            return false;

        string fieldName = predicate.Field;
        object fieldValue = predicate.Value;
        string operation = predicate.Operator.ToLower();

        var property = expense.GetType().GetProperty(fieldName);
        if (property == null) return false;

        var propertyValue = property.GetValue(expense);
        if (propertyValue == null) return false;

        switch (operation)
        {
            case "equal":
                return propertyValue.ToString().Equals(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
            case "notequal":
                return !propertyValue.ToString().Equals(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
            case "contains":
                return propertyValue.ToString().IndexOf(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase) >= 0;
            case "startswith":
                return propertyValue.ToString().StartsWith(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
            case "endswith":
                return propertyValue.ToString().EndsWith(fieldValue.ToString(), StringComparison.OrdinalIgnoreCase);
            case "greaterthan":
                return Convert.ToDouble(propertyValue) > Convert.ToDouble(fieldValue);
            case "lessthan":
                return Convert.ToDouble(propertyValue) < Convert.ToDouble(fieldValue);
            case "greaterthanorequal":
                return Convert.ToDouble(propertyValue) >= Convert.ToDouble(fieldValue);
            case "lessthanorequal":
                return Convert.ToDouble(propertyValue) <= Convert.ToDouble(fieldValue);
            default:
                return false;
        }
    }

    /// <summary>
    /// Applies grouping to the data source based on the specified group fields.
    /// Supports hierarchical/multi-level grouping.
    /// </summary>
    /// <param name="dataSource">The list of expense records to group.</param>
    /// <param name="groupFields">The list of field names to group by.</param>
    /// <returns>A sorted list of expense records organized by the group fields.</returns>
    private List<ExpenseRecord> ApplyGrouping(List<ExpenseRecord> dataSource, List<string> groupFields)
    {
        if (groupFields == null || groupFields.Count == 0)
            return dataSource;

        var sortedData = dataSource;
        foreach (var groupField in groupFields)
        {
            var property = typeof(ExpenseRecord).GetProperty(groupField);
            if (property != null)
            {
                sortedData = sortedData.OrderBy(x => property.GetValue(x) ?? string.Empty).ToList();
            }
        }

        return sortedData;
    }
}

/// <summary>
/// Represents the response structure for expense record queries.
/// </summary>
public class ExpenseRecordDataResponse
{
    /// <summary>
    /// Gets or sets the total count of records.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the list of expense record records.
    /// </summary>
    public List<ExpenseRecord> Result { get; set; } = new List<ExpenseRecord>();
}