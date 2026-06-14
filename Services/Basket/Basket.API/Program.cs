using System.Reflection;
using Basket.Application.Handlers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// add swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register mediatr
var assemblies = new Assembly[]
{
	Assembly.GetExecutingAssembly(),
	typeof(CreateShoppingCartHandler).Assembly,
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// add applications services
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// options pattern for cache settings
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));

// redis
builder.Services.AddStackExchangeRedisCache(
	(options) =>
	{
		options.Configuration = builder
			.Configuration.GetSection("CacheSettings")
			.GetValue<string>("ConnectionString");
	}
);

var app = builder.Build();

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
