using System.Reflection;
using Basket.Application.GrpcService;
using Basket.Application.Handlers;
using Basket.Application.Settings;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using static Discount.Grpc.Protos.DiscountProtoService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// built-in (Microsoft's) openapi/swagger support
builder.Services.AddOpenApi(); // microsoft style openapi

// add swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // swashbuckle style openapi

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
builder.Services.Configure<GrpcSettings>(builder.Configuration.GetSection("GrpcSettings"));

// register GRPC client using IOptions
builder.Services.AddGrpcClient<DiscountProtoServiceClient>(
	(sp, cfg) =>
	{
		var grpcSetting = sp.GetRequiredService<IOptions<GrpcSettings>>().Value;
		cfg.Address = new Uri(grpcSetting.DiscountUrl);
	}
);

// GRPC service
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoServiceClient>(
	(sp, cfg) =>
	{
		var grpcSetting = sp.GetRequiredService<IOptions<GrpcSettings>>().Value;
		cfg.Address = new Uri(grpcSetting.DiscountUrl);
	}
);

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

// todo: Use either swashbuckle or the built-in openapi support, not both.
// todo: keep swashbuckle for swagger UI
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi(); // microsoft style openapi
}

// enable swagger
app.UseSwagger(); // swashbuckle style openapi
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
