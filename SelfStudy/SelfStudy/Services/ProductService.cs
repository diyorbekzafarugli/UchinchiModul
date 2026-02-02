using SelfStudy.Dtos;
using SelfStudy.Entities;
using SelfStudy.Repositories;

namespace SelfStudy.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;
    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }
    public Guid? Create(ProductCreateDto dto)
    {
        if (dto.Price < 0 || string.IsNullOrWhiteSpace(dto.Name)) return null;
        Product product = new Product()
        {
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity,
        };

        return _repository.Add(product);
    }

    public bool Delete(Guid id)
    {
        return _repository.Delete(id);
    }

    public ProductResultDto? Get(Guid id)
    {
        var product = _repository.GetById(id);
        if (product == null) return null;

        var productDto = new ProductResultDto()
        {
            Name = product.Name,
            Price = product.Price,
            Id = product.Id
        };
        return productDto;
    }

    public List<ProductResultDto> GetAll()
    {
        var productsDto = new List<ProductResultDto>();
        var products = _repository.GetAll();

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

    public List<ProductResultDto> ProductsByName(string name)
    {
        var result = _repository.GetAll();
        var productsResult = new List<ProductResultDto>();

        foreach (var product in result)
        {

            if (product.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var resultDto = new ProductResultDto();
                resultDto.Id = product.Id;
                resultDto.Name = product.Name;
                resultDto.Price = product.Price;

                productsResult.Add(resultDto);
            }
        }
        return productsResult;
    }

    public List<ProductResultDto> ProductsExspensiveThen(decimal minPrace)
    {
        var result = _repository.GetAll();
        var productsResult = new List<ProductResultDto>();

        foreach (var product in result)
        {
            if (product.Price > minPrace)
            {
                var resultDto = new ProductResultDto();
                resultDto.Id = product.Id;
                resultDto.Name = product.Name;
                resultDto.Price = product.Price;

                productsResult.Add(resultDto);
            }
        }
        return productsResult;
    }

    public bool Update(ProductUpdateDto dto)
    {
        var product = _repository.GetById(dto.Id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;

        return _repository.Update(product);
    }
}
