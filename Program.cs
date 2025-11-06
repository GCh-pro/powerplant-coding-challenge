using Mapper;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Ajoute ton mapper dans le conteneur DI
builder.Services.AddScoped<IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>>, PowerPlantRequestMapper>();


builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
