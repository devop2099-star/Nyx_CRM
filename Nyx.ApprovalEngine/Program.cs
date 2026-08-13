using Nyx.ApprovalEngine.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register Database Factory
builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

// Configure Dapper snake_case mapping
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// Health Check Endpoint
app.MapGet("/health", () => Results.Ok(new 
{ 
    service = "Nyx Approval Engine", 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Engine Info Endpoint
app.MapGet("/api/engine/info", () => Results.Ok(new
{
    engine = "Nyx Approval Autonomous Decision Engine",
    capabilities = new[] { 
        "Multinivel Department Sign-Offs", 
        "Parallel Approval Evaluation", 
        "Granular Blocking Flags (progress, commission, activation, liquidation)", 
        "Decoupled Entity Support (Order, Lead, Budget, etc.)" 
    }
}));

app.MapControllers();

app.Run();
