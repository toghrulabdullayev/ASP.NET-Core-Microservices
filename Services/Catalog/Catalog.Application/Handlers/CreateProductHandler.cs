using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
	public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
	{
		private readonly IProductRepository _productRepository;

		public CreateProductHandler(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<ProductResponse> Handle(
			CreateProductCommand request,
			CancellationToken cancellationToken
		)
		{
			// fetch brand and type from repository
			var brand = await _productRepository.GetBrandById(request.BrandId);
			var type = await _productRepository.GetTypeById(request.TypeId);

			if (brand == null || type == null)
			{
				throw new ApplicationException("Invalid brand or type specified");
			}

			// match to entity
			var productEntity = request.ToEntity(brand, type);
			var newProduct = await _productRepository.CreateProduct(productEntity);
			return newProduct.ToResponse();
		}
	}
}
