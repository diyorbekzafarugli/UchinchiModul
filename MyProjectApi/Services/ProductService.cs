using MyProject.Dtos;
using MyProject.Entities;
using MyProject.Interfaces;

namespace MyProject.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;
    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }
    public Guid? Add(ProductCreateDto productDto)
    {
        if (productDto.Price < 0 || string.IsNullOrWhiteSpace(productDto.Name)) return null;

        var product = new Product()
        {
            Name = productDto.Name,
            Price = productDto.Price
        };
        return _repository.Add(product);
    }

    public bool Delete(Guid id)
    {
        if (id == Guid.Empty) return false;

        return _repository.Delete(id);
    }

    public List<ProductResultDto> GetAll()
    {
        var products = _repository.GetAll();
        if (products.Count == 0) return new List<ProductResultDto>();

        var productsDto = new List<ProductResultDto>();
        foreach (var product in products)
        {
            var productDto = new ProductResultDto()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
            productsDto.Add(productDto);
        }
        return productsDto;
    }

    public ProductResultDto? GetById(Guid id)
    {
        if (id == Guid.Empty) return null;

        var product = _repository.GetById(id);
        if (product is null) return null;

        var resultDto = new ProductResultDto()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
        return resultDto;
    }

    public bool Update(ProductUpdateDto productUpdateDto)
    {
        if (productUpdateDto.Id == Guid.Empty || productUpdateDto.Price < 0 || string.IsNullOrWhiteSpace(productUpdateDto.Name)) return false;
        var product = new Product()
        {
            Id = productUpdateDto.Id,
            Name = productUpdateDto.Name,
            Price = productUpdateDto.Price
        };
        return _repository.Update(product);
    }
}
