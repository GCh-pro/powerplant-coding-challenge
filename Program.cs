using Mapper;
using Models;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Ajoute ton mapper dans le conteneur DI
builder.Services.AddScoped<IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>>, PowerPlantRequestMapper>();

// Register the service layer so it can be injected into controllers
// Register ProductionPlanner implementation for the IProductionPlanner interface
builder.Services.AddScoped<IProductionPlanner, ProductionPlanner>();


builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

// Expose a public Program type for integration tests (WebApplicationFactory requires a host entry type)
public partial class Program { }
