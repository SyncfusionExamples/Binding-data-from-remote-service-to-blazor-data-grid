using GraphQLServer.GraphQL;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register GraphQL services
builder.Services.AddGraphQLServer()
    .AddQueryType<GraphQLQuery>()
    .AddMutationType<GraphQLMutation>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:7149")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowSpecificOrigin");

//// Log incoming GraphQL requests
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.StartsWithSegments("/graphql"))
//    {
//        context.Request.EnableBuffering(); // Enable multiple reads
//        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
//        context.Request.Body.Position = 0;

//        Console.WriteLine("==== Incoming GraphQL Request ====");
//        Console.WriteLine(body);
//    }

//    await next();
//});

// Use routing
app.UseRouting();

// Map endpoints
app.MapGet("/", () => "Hello World!");
app.MapGraphQL(); // this maps /graphql by default

app.Run();
