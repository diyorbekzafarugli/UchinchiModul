using MyProductAPI.Dtos;
using MyProductAPI.Entities;
using MyProductAPI.Repositories;

namespace MyProductAPI.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;
    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }
    public Guid? Create(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0 || dto.Quantity < 0) return null;
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
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
                Price = product.Price,
                Quantity = product.Quantity,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
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
        var productDto = new ProductResultDto()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
        return productDto;
    }

    public bool Update(UpdateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price < 0 || dto.Quantity < 0 || dto.Id == Guid.Empty) return false;
        var product = _repository.GetById(dto.Id);

        if (product is null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.Quantity = dto.Quantity;
        product.UpdatedAt = DateTime.UtcNow;
        
        return _repository.Update(product);
    }
}
