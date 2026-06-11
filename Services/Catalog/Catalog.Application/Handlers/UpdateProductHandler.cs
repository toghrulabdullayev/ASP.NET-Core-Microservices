using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
	public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
	{
		private readonly IProductRepository _productRepository;

		public UpdateProductHandler(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<bool> Handle(
			UpdateProductCommand request,
			CancellationToken cancellationToken
		)
		{
			var existing = await _productRepository.GetProduct(request.Id);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Product with ID {request.Id} not found");
			}

			// fetch brand and type from repository
			var brand = await _productRepository.GetBrandById(request.BrandId);
			var type = await _productRepository.GetTypeById(request.TypeId);

			if (brand == null || type == null)
			{
				throw new ApplicationException("Invalid brand or type specified");
			}

			// mapper role
			var updatedProduct = request.ToUpdatedEntity(existing, brand, type);

			// save the record
			return await _productRepository.UpdateProduct(updatedProduct);
		}
	}
}
