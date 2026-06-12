using Catalog.Application.Commands;
using Catalog.Application.DTOs;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
	[ApiController] // Base route for the controller, using the class name as part of the route
	[Route("api/v1/[controller]")] // api/v1/catalog (based on the class name)
	public class CatalogController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CatalogController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("GetAllProducts")] // api/v1/catalog/GetAllProducts
		public async Task<ActionResult<IList<ProductDto>>> GetAllProducts(
			[FromQuery] CatalogSpecParams catalogSpecParams // Bind query parameters to CatalogSpecParams
		)
		{
			var query = new GetAllProductsQuery(catalogSpecParams);
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("{id}")] // api/v1/catalog/{id}
		public async Task<ActionResult<ProductDto>> GetProduct(string id)
		{
			var query = new GetProductByIdQuery(id);
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("productName/{productName}")] // api/v1/catalog/productName/{productName}
		public async Task<ActionResult<IList<ProductDto>>> GetProductByName(string productName)
		{
			var query = new GetProductsByNameQuery(productName);
			var result = await _mediator.Send(query);
			if (result == null || !result.Any())
			{
				return NotFound();
			}

			var dtoList = result.Select(p => p.ToDto()).ToList();
			return Ok(dtoList);
		}

		[HttpPost] // api/v1/catalog
		public async Task<ActionResult<ProductDto>> CreateProduct(
			[FromBody] CreateProductDto createProductDto
		)
		{
			var command = createProductDto.ToCommand();
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpDelete("{id}")] // api/v1/catalog/{id}
		public async Task<IActionResult> DeleteProduct(string id)
		{
			var command = new DeleteProductByIdCommand(id);
			var result = await _mediator.Send(command);
			if (!result)
			{
				return NotFound();
			}

			return NoContent();
		}

		[HttpPut("{id}")] // api/v1/catalog/{id}
		public async Task<IActionResult> UpdateProduct(string id, UpdateProductDto updateProductDto)
		{
			var command = updateProductDto.ToCommand(id);
			var result = await _mediator.Send(command);
			if (!result)
			{
				return NotFound();
			}

			return NoContent();
		}

		[HttpGet("GetAllBrands")] // api/v1/catalog/GetAllBrands
		public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
		{
			var query = new GetAllBrandsQuery();
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("GetAllTypes")] // api/v1/catalog/GetAllTypes
		public async Task<ActionResult<IEnumerable<TypeDto>>> GetTypes()
		{
			var query = new GetAllTypesQuery();
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("/brand/{brand}", Name = "GetProductsByBrandName")] // api/v1/catalog/brand/{brand}
		public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrand(string brand)
		{
			var query = new GetProductsByBrandQuery(brand);
			var result = await _mediator.Send(query);
			return Ok(result);
		}
	}
}
