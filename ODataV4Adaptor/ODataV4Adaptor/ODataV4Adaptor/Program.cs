using ODataV4Adaptor.Client.Models;
using ODataV4Adaptor.Client.Pages;
using ODataV4Adaptor.Components;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Syncfusion.Blazor;

// Create a new instance of the web application builder.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSyncfusionBlazor();

// Create an ODataConventionModelBuilder to build the OData model.
var modelBuilder = new ODataConventionModelBuilder();

// Register the "Grid" entity set with the OData model builder.
modelBuilder.EntitySet<OrdersDetails>("Grid");

var recordCount = OrdersDetails.GetAllRecords().Count;

// Add controllers with OData support to the service collection
builder.Services.AddControllers().AddOData(
    options => options
        // Enables $count query option to retrieve total record count.
        .Count()
        // Enables $filter query option to allow filtering and allow searching based on field values.
        .Filter()
        // Enables $orderby query option to allow sorting based on field values.
        .OrderBy()
        // Limits the maximum number of records returned using $top.
        .SetMaxTop(recordCount)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel())
);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();
// Map controller routes.
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ODataV4Adaptor.Client._Imports).Assembly);

app.Run();
