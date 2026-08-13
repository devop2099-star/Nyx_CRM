using Nyx.CheckpointEngine.Persistence;

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
    service = "Nyx Checkpoint Engine", 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Engine Info Endpoint
app.MapGet("/api/engine/info", () => Results.Ok(new
{
    engine = "Nyx Checkpoint Autonomous State Engine",
    capabilities = new[] { 
        "Transition Rules Validation", 
        "Chained Triggering (trigger_on_ko)", 
        "Automatic Rollbacks", 
        "JSONB History Audit Logs" 
    }
}));

app.MapControllers();

app.Run();
