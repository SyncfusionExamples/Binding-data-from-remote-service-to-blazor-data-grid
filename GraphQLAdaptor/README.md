# Blazor DataGrid with Hot Chocolate GraphQL

## Project Overview

This repository demonstrates a production-ready pattern for binding **GraphQL** data to **Syncfusion Blazor DataGrid** using **Hot Chocolate GraphQL Server**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using GraphQL queries, mutations, resolvers, and a GraphQL adaptor for seamless grid functionality.

## Key Features

- **Hot Chocolate GraphQL Server Integration**: Query resolvers for read operations and mutation resolvers for write operations
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update expense records directly from the grid
- **GraphQL Adaptor**: Full control over grid data operations (read, search, filter, sort, page, group) via GraphQL queries and mutations
- **DataManagerRequestInput**: Structured input type for handling complex data operations from the DataGrid
- **Configurable GraphQL Endpoint**: Backend port and GraphQL endpoint managed via `launchSettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2022 | 17.0 or later | Development IDE with Blazor workload |
| .NET SDK | net8.0 or compatible | Runtime and build tools |
| HotChocolate.AspNetCore | 15.1 or later | GraphQL server framework |
| Syncfusion.Blazor.Grids | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |
| Syncfusion.Blazor.Data | Latest | Data adaptors including GraphQL support |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding-data-from-remote-service-to-blazor-data-grid"
   cd "GraphQLAdaptor/Grid_GraphQLAdaptor"
   ```

2. **Restore packages and build**
   ```powershell
   dotnet restore; dotnet build
   ```

3. **Run the application**
   ```powershell
   dotnet run
   ```

4. **Open the application**
   
   Navigate to the local URL displayed in the terminal (typically `https://localhost:7272` or `http://localhost:5272`).

5. **Access GraphQL Playground (Optional)**
   
   Navigate to `https://localhost:7272/graphql` to explore the GraphQL schema and test queries/mutations manually.

## Configuration

### GraphQL Endpoint Port Configuration

The GraphQL endpoint port is configured in `Properties/launchSettings.json`. This file controls where the application runs and where the GraphQL endpoint is exposed.

**Instructions to Change the Port:**

1. Open `Properties/launchSettings.json` in the project root.
2. Locate the `https` profile section:

```json
"https": {
  "commandName": "Project",
  "dotnetRunMessages": true,
  "launchBrowser": true,
  "applicationUrl": "https://localhost:7272;http://localhost:5272",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

3. Modify the `applicationUrl` property to change port numbers:
   - `https://localhost:7272` - HTTPS port for GraphQL endpoint
   - `http://localhost:5272` - HTTP port for GraphQL endpoint

4. Update the DataGrid connection URL in `Components/Pages/Home.razor` to match the configured port:
   ```html
   <SfDataManager Url="https://localhost:7272/graphql" GraphQLAdaptorOptions="@adaptorOptions" Adaptor="Adaptors.GraphQLAdaptor"></SfDataManager>
   ```

**Important Notes:**
- Port numbers must be between 1024 and 65535
- Avoid using ports already in use by other applications
- The GraphQL endpoint is always accessible at `/graphql` path (configured via `app.MapGraphQL()` in Program.cs)

### GraphQL Adaptor Configuration

The `GraphQLAdaptorOptions` in `Home.razor` defines how the DataGrid communicates with the GraphQL backend. This configuration specifies:

- **Query**: GraphQL query for reading expense records with data operations support
- **ResolverName**: Backend resolver method name (e.g., `expenseRecordData`)
- **Mutation.Insert**: GraphQL mutation for creating new records
- **Mutation.Update**: GraphQL mutation for updating existing records
- **Mutation.Delete**: GraphQL mutation for deleting records
- **Mutation.Batch**: GraphQL mutation for batch operations (multiple add/edit/delete)

Refer to the [Full Documentation](#full-documentation) for complete GraphQL adaptor configuration examples.

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Models/ExpenseRecord.cs` | Data model representing expense records |
| `/Models/GraphQLQuery.cs` | Query resolvers for read operations (GetExpenseRecordData) |
| `/Models/GraphQLMutation.cs` | Mutation resolvers for write operations (CreateExpense, UpdateExpense, DeleteExpense, BatchUpdate) |
| `/Models/DataManagerRequestInput.cs` | Input type for data operation parameters (filter, sort, search, paging) |
| `/Components/Pages/Home.razor` | DataGrid page with GraphQL adaptor configuration and expense management UI |
| `/Program.cs` | Service registration for Hot Chocolate GraphQL server |
| `/Properties/launchSettings.json` | Port configuration for GraphQL endpoint |

## Common Tasks

### Add an Expense

1. Click the **Add** button in the toolbar
2. Fill in the form fields (Employee Name, Amount, Category, Status, etc.)
3. Upload an employee avatar image (optional)
4. Click **Update** to create the record via GraphQL mutation
5. The system automatically generates a unique ExpenseId (e.g., EXP001)

### Edit an Expense

1. Select a row in the grid
2. Click the **Edit** button in the toolbar or double-click the row
3. Modify the required fields in the dialog
4. Click **Update** to save changes via GraphQL mutation
5. Total amount is automatically recalculated based on Amount and Tax Percentage

### Delete an Expense

1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion in the dialog
4. The record is removed via GraphQL mutation

### Search Records

1. Use the **Search** box in the toolbar
2. Enter keywords to filter records (searches across all columns)
3. The search query is sent to the backend via GraphQL with field specifications

### Filter Records

1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply
4. The filter conditions are sent to the backend as GraphQL predicate objects

### Sort Records

1. Click the column header to sort ascending
2. Click again to sort descending
3. Hold **Ctrl** and click additional headers for multi-level sorting
4. Sort specifications are sent to the backend via GraphQL

### Group Records

1. Drag a column header to the group drop area above the grid
2. Click the group header to expand or collapse groups
3. Group fields are sent to the backend via the GraphQL data manager request

### Batch Operations

1. Make multiple changes to different rows (add, edit, delete)
2. Click **Update** to apply all changes in a single operation
3. The batch mutation sends all changed, added, and deleted records to the backend in one request

## Troubleshooting

### GraphQL Endpoint Not Responding

- Verify the application is running: Check terminal for `Now listening on: https://localhost:xxxx`
- Confirm the URL in `SfDataManager` matches the running port
- Ensure `app.MapGraphQL()` is configured in `Program.cs`
- Check firewall settings if accessing from a different machine

### Static Files Not Loading (CSS/Scripts)

- Verify Syncfusion stylesheet is referenced in `Components/App.razor`:
  ```html
  <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
  ```
- Verify Syncfusion scripts are referenced in `Components/App.razor`:
  ```html
  <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js" type="text/javascript"></script>
  ```
- Check browser developer tools (F12) for 404 errors on static resources

### CRUD Operations Not Working

- Verify the GraphQL endpoint URL in `Home.razor` SfDataManager matches the backend port
- Check browser console for GraphQL errors (Network tab → GraphQL requests)
- Ensure all GraphQL mutation resolvers are implemented in `GraphQLMutation.cs`
- Verify the `DataManagerRequestInput` class includes all required properties for data operations

### Version Conflicts

- Align HotChocolate.AspNetCore and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `Grid_GraphQLAdaptor.csproj` file for conflicting version constraints
- Verify all packages are compatible with .NET 8.0 or later

### Port Already in Use

- Change the port numbers in `launchSettings.json` to available ports
- Or identify the process using the port: `Get-Process | Where-Object {$_.Handles -gt 500} | Select-Object Name, Id`
- Update the DataGrid connection URL to match the new port

## Full Documentation

Detailed, step-by-step directions including complete code examples, GraphQL query structures, mutation implementations, and advanced configurations are available in the [user guide](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-adaptors/graphql-adaptor).
