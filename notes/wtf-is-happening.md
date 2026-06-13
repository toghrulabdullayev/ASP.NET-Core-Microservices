What you have is essentially **Clean Architecture + DDD + CQRS**, where **Catalog.Core acts as the Domain layer**. Many .NET developers call it `Core` instead of `Domain` because it contains the most important business concepts and abstractions. ([MiniMini Blog][1])

Your dependency flow looks like this:

```text
Catalog.API
    ↓
Catalog.Application
    ↓
Catalog.Core
    ↑
Catalog.Infrastructure
```

Notice that **Infrastructure depends on Core**, not the other way around. This is one of the main principles of Clean Architecture. ([TatvaSoft][2])

---

# 1. Catalog.Core (Domain Layer)

This is the **heart of the application**.

```text
Catalog.Core
├── Entities
├── Repositories
└── Specifications
```

The Core layer should know nothing about:

- MongoDB
- HTTP
- ASP.NET
- Controllers
- Swagger
- External APIs

It only knows business concepts. ([c-sharpcorner.com][3])

---

## Entities

```text
Entities
├── BaseEntity.cs
├── Product.cs
├── ProductBrand.cs
└── ProductType.cs
```

These represent real business objects.

Example:

```csharp
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public string BrandId { get; set; }
    public string TypeId { get; set; }
}
```

Think:

> "What things exist in my business?"

Products, Brands, Types, Orders, Customers, etc.

Those belong here.

---

## Repositories

```text
Repositories
├── IBrandRepository.cs
├── IProductRepository.cs
└── ITypeRepository.cs
```

These are contracts.

Example:

```csharp
public interface IProductRepository
{
    Task<Product> GetProduct(string id);
}
```

Core says:

> "I need a way to get products."

Core does **not** say:

> "Use MongoDB."

Infrastructure later provides the implementation. ([Reddit][4])

---

## Specifications

```text
Specifications
├── CatalogSpecParams.cs
└── Pagination.cs
```

Used to describe queries.

Instead of:

```csharp
GetProducts(
    string brand,
    string type,
    int page,
    int pageSize
)
```

you create:

```csharp
CatalogSpecParams
{
    BrandId,
    TypeId,
    PageIndex,
    PageSize
}
```

which represents filtering requirements.

---

# 2. Catalog.Application (Use Cases Layer)

This layer answers:

> "What can the user do?"

Examples:

- Get all products
- Get product by ID
- Create product
- Delete product

Application orchestrates business operations. It does not care about MongoDB or HTTP. ([cleanarchitecture.jasontaylor.dev][5])

---

## Queries

```text
Queries
├── GetAllBrandsQuery.cs
└── GetAllTypesQuery.cs
```

A Query represents a read operation.

Example:

```csharp
public record GetAllBrandsQuery()
    : IRequest<IEnumerable<BrandResponse>>;
```

Think:

> "I want all brands."

Nothing happens yet.

This is just the request.

---

## Handlers

```text
Handlers
├── GetAllBrandsHandler.cs
└── GetAllTypesHandler.cs
```

Handlers execute the query.

Example:

```csharp
public class GetAllBrandsHandler
    : IRequestHandler<GetAllBrandsQuery,
        IEnumerable<BrandResponse>>
{
    private readonly IBrandRepository _repository;

    public async Task<IEnumerable<BrandResponse>>
        Handle(...)
    {
        var brands = await _repository.GetAllAsync();

        return brands.Select(...);
    }
}
```

Flow:

```text
Controller
    ↓
Query
    ↓
Handler
    ↓
Repository
    ↓
Database
```

Handler = the actual use case implementation. ([Mintlify][6])

---

## CQRS View

Your project is already using MediatR and CQRS.

```text
Query = Read

Command = Write
```

Examples:

```text
GetProductQuery
GetAllProductsQuery
```

read data.

```text
CreateProductCommand
DeleteProductCommand
UpdateProductCommand
```

change data.

Each gets its own Handler. ([Mintlify][6])

---

## DTOs

```text
DTOs
└── ProductDto.cs
```

DTO = Data Transfer Object

Used to move data between layers.

Instead of returning:

```csharp
Product
```

you return:

```csharp
ProductDto
```

because the API shouldn't expose the entire domain object.

---

## Responses

```text
Responses
├── ProductResponse.cs
├── BrandResponse.cs
└── TypeResponse.cs
```

These define what the API sends back.

Example:

```csharp
public record ProductResponse(
    string Id,
    string Name,
    decimal Price
);
```

---

## Mappers

```text
Mappers
├── BrandMapper.cs
└── TypeMapper.cs
```

Mappers convert between objects.

Example:

```csharp
Product
    ↓
ProductResponse
```

```csharp
public static ProductResponse ToResponse(
    Product product)
{
    return new ProductResponse(
        product.Id,
        product.Name,
        product.Price);
}
```

Without mappers you'd have conversion code scattered everywhere.

Think:

> Entity ↔ DTO ↔ Response

Mappers handle the translation. ([Mintlify][6])

---

# 3. Catalog.Infrastructure

This is where reality happens 😄

```text
Catalog.Infrastructure
├── Repositories
├── Data
└── Settings
```

Infrastructure talks to:

- MongoDB
- Redis
- RabbitMQ
- Email services
- File systems
- External APIs

etc. ([TatvaSoft][2])

---

## Repository Implementations

```text
Repositories
├── ProductRepository.cs
├── BrandRepository.cs
└── TypeRepository.cs
```

Core defined:

```csharp
IProductRepository
```

Infrastructure implements it:

```csharp
public class ProductRepository
    : IProductRepository
{
    private readonly IMongoCollection<Product>
        _products;
}
```

This is where MongoDB code belongs.

---

## Data

```text
Data
├── DatabaseSeeder.cs
└── SeedData
```

Used for:

```text
brands.json
products.json
types.json
```

Initial database population.

When app starts:

```text
JSON
  ↓
Seeder
  ↓
MongoDB
```

---

## Settings

```text
Settings
└── DatabaseSettings.cs
```

Configuration objects.

Example:

```csharp
public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
```

Maps from:

```json
{
	"DatabaseSettings": {
		"ConnectionString": "...",
		"DatabaseName": "CatalogDb"
	}
}
```

---

# 4. Catalog.API

This is the presentation layer.

```text
Catalog.API
├── Program.cs
├── appsettings.json
└── Controllers (likely coming soon)
```

Responsibilities:

- HTTP requests
- Routing
- Swagger
- Authentication
- Dependency Injection

Very thin.

Example:

```csharp
[HttpGet]
public async Task<IActionResult> GetBrands()
{
    var result =
        await _mediator.Send(
            new GetAllBrandsQuery());

    return Ok(result);
}
```

Controller should not contain business logic.

It just forwards work to Application. ([dddsample.sourceforge.net][7])

---

# Complete Request Flow

When someone calls:

```http
GET /api/brands
```

the flow becomes:

```text
API
│
├─ Controller
│
▼
GetAllBrandsQuery
│
▼
GetAllBrandsHandler
│
▼
IBrandRepository
│
▼
BrandRepository
│
▼
MongoDB
│
▼
Brands
│
▼
BrandMapper
│
▼
BrandResponse
│
▼
API Response
```

That's the entire journey of a request through your architecture.

---

One thing I'd suggest: once you start adding more features, consider organizing Application by **feature folders** instead of separate `Handlers`, `Queries`, `Responses`, and `Mappers` folders. Many modern .NET projects use:

```text
Application
└── Products
    ├── GetProducts
    │   ├── GetProductsQuery.cs
    │   ├── GetProductsHandler.cs
    │   └── ProductResponse.cs
    │
    └── CreateProduct
        ├── CreateProductCommand.cs
        ├── CreateProductHandler.cs
        └── CreateProductValidator.cs
```

This "vertical slice" approach tends to scale better as the project grows. ([cleanarchitecture.jasontaylor.dev][5])

[1]: https://www.minimini.io/blog/clean-architecture-dotnet-8-domain-application-infrastructure?utm_source=chatgpt.com "Clean Architecture in .NET 8: Domain, Application & Infrastructure | MiniMini Blog"
[2]: https://www.tatvasoft.com/blog/clean-architecture-net-core/?utm_source=chatgpt.com "Clean Architecture .NET Core - TatvaSoft Blog"
[3]: https://www.c-sharpcorner.com/article/clean-architecture-in-asp-net-co-core-concept-and-guide/?utm_source=chatgpt.com "Clean Architecture in ASP.NET Core: Core Concept and Guide"
[4]: https://www.reddit.com/r/dotnet/comments/ga02lh?utm_source=chatgpt.com "Question about application core and infrastructure layer in onion/clean architecture"
[5]: https://cleanarchitecture.jasontaylor.dev/docs/architecture/application-layer/?utm_source=chatgpt.com "Application layer | Clean Architecture Solution Template"
[6]: https://www.mintlify.com/FedeJG82/HybridArchitecture.NET/architecture/layers?utm_source=chatgpt.com "Architecture Layers - Hybrid DDD Architecture .NET"
[7]: https://dddsample.sourceforge.net/architecture.html?utm_source=chatgpt.com "DDD Sample Application - Architecture"

You're asking exactly the right questions. When people first see CQRS + Clean Architecture, it often feels like:

> "Why do I need 3 files when 1 file would work?"

The answer is: **you don't always need them**. These patterns exist to keep large codebases maintainable, not because the compiler demands them.

---

## 1. Why have DTOs and Responses separately?

Short answer:

**They can be the same thing.**

In many projects, you'll see:

```csharp
public record ProductDto(...);
```

and that's the object returned directly from the API.

No separate `ProductResponse` exists.

---

### When they're different

A DTO is a generic data-transfer object used between layers.

```csharp
Product
    ↓
ProductDto
```

A Response is specifically an API contract.

```csharp
ProductDto
    ↓
ProductResponse
```

Example:

```csharp
public record ProductDto(
    string Id,
    string Name,
    decimal Price,
    decimal CostPrice
);
```

But API consumers should not see `CostPrice`.

```csharp
public record ProductResponse(
    string Id,
    string Name,
    decimal Price
);
```

Then:

```csharp
Entity
    ↓
DTO
    ↓
Response
```

---

### In your project?

Honestly?

Your current:

```csharp
ProductDto
BrandDto
TypeDto
```

already look like API response models.

If I were reviewing this code, I'd probably ask:

> "Why do we need ProductResponse at all?"

Unless you're planning to expose different shapes of data to different consumers.

Many teams simply pick one:

```text
DTOs
```

or

```text
Responses
```

and remove the other. ([Nippysoft S.A.S][1])

---

## 2. Why have separate Queries and Handlers?

Because CQRS treats a request and its execution as different things.

Consider:

```csharp
GetAllBrandsQuery
```

This is just a message.

```csharp
public record GetAllBrandsQuery()
    : IRequest<IEnumerable<BrandDto>>;
```

No logic.

No database.

No repository.

Just:

> "Please give me all brands."

---

The Handler contains the actual implementation:

```csharp
public class GetAllBrandsHandler
    : IRequestHandler<GetAllBrandsQuery,
        IEnumerable<BrandDto>>
{
    ...
}
```

This contains:

```csharp
_repository.GetAllAsync()
```

mapping

validation

business rules

etc.

---

Think of it like this:

### Query

```text
What do you want?
```

### Handler

```text
How do you get it?
```

---

Without MediatR you'd write:

```csharp
var products =
    await productService.GetAllProducts();
```

With MediatR:

```csharp
await _mediator.Send(
    new GetAllProductsQuery()
);
```

MediatR finds the handler automatically. ([c-sharpcorner.com][2])

---

### Why does the Query seem empty?

Because that's normal.

A query is often just:

```csharp
public record GetProductQuery(string Id);
```

or

```csharp
public record GetProductsQuery(
    int Page,
    int PageSize
);
```

It's basically a typed request object.

The value comes later when you add:

```text
GetProductsQuery
GetProductsHandler

GetProductQuery
GetProductHandler

GetProductsByBrandQuery
GetProductsByBrandHandler
```

Now every use case has one obvious home. ([Coding Droplets][3])

---

## 3. Does a Mapper translate Entity → DTO/Response?

Yes. Exactly.

Without mappers:

```csharp
public async Task<ProductDto> Handle(...)
{
    var product =
        await _repository.GetById(id);

    return new ProductDto(
        product.Id,
        product.Name,
        ...
    );
}
```

The handler now contains mapping code.

---

With a mapper:

```csharp
var dto = product.ToDto();
```

or

```csharp
var dto =
    ProductMapper.ToDto(product);
```

The handler focuses only on the use case.

```csharp
var product =
    await _repository.GetById(id);

return ProductMapper.ToDto(product);
```

---

Think of mappers as translators:

```text
MongoDB Entity
        ↓
Domain Entity
        ↓
DTO
        ↓
Response
```

Every layer speaks its own language.

The mapper translates between them.

---

### One thing I'd personally change

For a project your size, I'd probably have:

```text
Application
├── DTOs
├── Commands
├── Queries
└── Handlers
```

and delete:

```text
Responses
```

unless you have a concrete reason for separate API contracts.

Right now your `ProductDto` already looks like what I would return from the API, so `ProductResponse` may just be an extra layer without much benefit. That's a fairly common observation in CQRS projects as teams try to balance architectural purity against complexity. ([c-sharpcorner.com][4])

So your understanding is already close to the mark:

✅ **Query = request definition**
✅ **Handler = implementation of that request**
✅ **Mapper = translation between layers**
✅ **DTO and Response often overlap and can sometimes be merged**

The only thing I'd add is that in mature CQRS systems, developers often stop thinking in terms of "services" and start thinking in terms of **use cases**:

```text
GetProduct
CreateProduct
UpdateProduct
DeleteProduct
```

Each use case gets:

```text
Command/Query
Handler
Validator
Response
```

and lives independently from the others. That's the real idea behind the pattern.

[1]: https://nippysoft.com/en/blog/arquitectura-de-software/cqrs-dotnet-10-implementacion-mediatr-csharp-14-2026?utm_source=chatgpt.com "CQRS in .NET 10 with MediatR and C# 14 - NippySoft"
[2]: https://www.c-sharpcorner.com/article/cqrs-mediatr/?utm_source=chatgpt.com "CQRS ≠ MediatR"
[3]: https://codingdroplets.com/clean-architecture-cqrs-mediatr-aspnet-core-2026?utm_source=chatgpt.com "Clean Architecture with CQRS and MediatR in ASP.NET Core (2026)"
[4]: https://www.c-sharpcorner.com/article/how-do-cqrs-and-mediatr-improve-architecture-in-asp-net-core-web-apis/Default.aspx?utm_source=chatgpt.com "How Do CQRS and MediatR Improve Architecture in ASP.NET Core Web APIs"

Exactly 👍

Think of it like this:

### Without MediatR

Your controller directly calls a service:

```csharp
[HttpGet]
public async Task<IActionResult> GetProducts(
    string? brandId,
    string? typeId)
{
    var products = await _productService.GetProducts(
        brandId,
        typeId);

    return Ok(products);
}
```

Flow:

```text
Controller
    ↓
ProductService
    ↓
Repository
```

---

### With MediatR

Instead of calling a service method, you create a request object:

```csharp
public record GetProductsQuery(
    string? BrandId,
    string? TypeId
) : IRequest<IEnumerable<ProductDto>>;
```

Then:

```csharp
[HttpGet]
public async Task<IActionResult> GetProducts(
    string? brandId,
    string? typeId)
{
    var result = await _mediator.Send(
        new GetProductsQuery(
            brandId,
            typeId));

    return Ok(result);
}
```

MediatR finds the matching handler automatically through DI and generic type matching. ([Stack Overflow][1])

Then:

```csharp
public class GetProductsHandler
    : IRequestHandler<
        GetProductsQuery,
        IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>>
        Handle(
            GetProductsQuery query,
            CancellationToken ct)
    {
        // filtering
        // repository calls
        // mapping
        // business rules

        return products;
    }
}
```

The flow becomes:

```text
Controller
    ↓
GetProductsQuery
    ↓
MediatR
    ↓
GetProductsHandler
    ↓
Repository
    ↓
MongoDB
```

([aicodesnippet.com][2])

---

And yes, the query object often contains the filters:

```csharp
public record GetProductsQuery(
    string? BrandId,
    string? TypeId,
    int Page = 1,
    int PageSize = 10
);
```

Then the handler decides what to do with those values:

```csharp
public async Task<IEnumerable<ProductDto>>
    Handle(
        GetProductsQuery query,
        CancellationToken ct)
{
    var spec = new CatalogSpecParams
    {
        BrandId = query.BrandId,
        TypeId = query.TypeId,
        PageIndex = query.Page,
        PageSize = query.PageSize
    };

    var products =
        await _repository.GetProducts(spec);

    return products.Select(ProductMapper.ToDto);
}
```

---

One small correction to your wording:

> "we put the query class inside the mediatr send call, and it executes the right handler"

Not quite "the query executes the handler."

More accurately:

```text
Controller
    ↓
creates Query object
    ↓
Mediator receives Query
    ↓
Mediator locates matching Handler
    ↓
Handler executes
```

The query is just a data container/request message. It contains no behavior. The handler contains the behavior. ([Medium][3])

So if you see:

```csharp
await _mediator.Send(
    new GetProductsQuery(...)
);
```

you can mentally read it as:

> "Hey MediatR, here's a request to get products. Find whoever knows how to do that and execute it."

And the handler is that "whoever." 😄

[1]: https://stackoverflow.com/questions/73605982/how-does-mediatr-know-which-handler-to-call?utm_source=chatgpt.com "c# - How does MediatR know which handler to call? - Stack Overflow"
[2]: https://www.aicodesnippet.com/c-sharp-tutorials/frameworks-and-libraries/mediatr-for-in-process-messaging-cqrs.html?utm_source=chatgpt.com "C# tutorials : MediatR for in-process messaging (CQRS)"
[3]: https://medium.com/%40juancamba/difference-between-queries-and-commands-in-mediatr-cc53448fd55b?utm_source=chatgpt.com "Difference Between Queries and Commands in MediatR | by Juan Camba | Medium"
