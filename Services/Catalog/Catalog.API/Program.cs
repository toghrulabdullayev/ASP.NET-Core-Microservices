using System.Reflection;
using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

//? register custom serializers for MongoDB to handle specific data types like Guid, decimal, and DateTimeOffset
// "id": "123e4567-e89b-12d3-a456-426614174000" instead of "id": {"$binary": "123e4567-e89b-12d3-a456-426614174000"}
// "price": 19.99 instead of "price": {"$numberDecimal": "19.99"}
// "createdDate": "2024-06-01T12:00:00Z" instead of "createdDate": {"$date": "2024-06-01T12:00:00Z"}
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// add swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register mediatr
var assemblies = new Assembly[]
{
	Assembly.GetExecutingAssembly(), // Catalog.API assembly
	typeof(GetAllBrandsHandler).Assembly, // Catalog.Application assembly (contains handlers)
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// custom services
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// bind settings from appsettings.json to DatabaseSettings class and register it with the DI container
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

// register MongoClient as a singleton service, using the connection string from the DatabaseSettings
builder.Services.AddSingleton<IMongoClient>(sp =>
{
	var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
	return new MongoClient(settings.ConnectionString);
});

var app = builder.Build();

// seed the database with initial data if it's empty
using (var scope = app.Services.CreateScope())
{
	var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();
	await DatabaseSeeder.SeedAsync(options);
}

// configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

// enable swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
