using Product.Api.Dtos;
using Product.Api.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Services;

public class ProductService : IProductService
{
    private readonly IRepositoriy<Entities.Product> _repository;
    public ProductService(IRepositoriy<Entities.Product> repositoriy)
    {
        _repository = repositoriy;
    }

    public Guid Create(CreateProductDto createProductDto)
    {
        if (createProductDto.Name.Length < 3 || createProductDto.Price <= 0 || createProductDto.Stock < 0 || createProductDto.Category.Length == 0) return Guid.Empty;
        Entities.Product product = new Entities.Product()
        {
            Id = Guid.NewGuid(),
            Name = createProductDto.Name,
            Category = createProductDto.Category,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
        return _repository.Add(product);
    }

    public bool Delete(Guid id)
    {
        if (id == Guid.Empty) return false;
        return _repository.Delete(id);
    }

    public List<ResultProductDto> GetAll()
    {
        var products = _repository.GetAll();
        if (products.Count == 0) return new List<ResultProductDto>();
        var resultProduct = new List<ResultProductDto>();

        foreach (var product in products)
        {
            var productDto = new ResultProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category
            };
            resultProduct.Add(productDto);
        }
        return resultProduct;
    }

    public ResultProductDto? GetById(Guid id)
    {
        var product = _repository.GetById(id);
        if (product is null) return null;

        var resultProduct = new ResultProductDto()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category
        };
        return resultProduct;
    }

    public bool Update(Guid id, UpdateProductDto updateProductDto)
    {
        if(id != updateProductDto.Id) return false;
        if (updateProductDto.Name.Length < 3 || updateProductDto.Price <= 0 || updateProductDto.Stock < 0 || updateProductDto.Category.Length == 0) return false;
        var product = _repository.GetById(id);
        if (product is null) return false;

        product.Name = updateProductDto.Name;
        product.Price = updateProductDto.Price;
        product.Category = updateProductDto.Category;
        product.Stock = updateProductDto.Stock;
        product.UpdatedAt = DateTime.UtcNow;
        _repository.Update(product);
        return true;
    }
}
