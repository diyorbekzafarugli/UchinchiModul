using Product.Api.Dtos;

namespace Product.Api.Services;

public interface IProductService
{
    Guid Create(CreateProductDto createProductDto);
    List<ResultProductDto> GetAll();
    ResultProductDto? GetById(Guid id);
    bool Update(Guid id, UpdateProductDto updateProductDto);
    bool Delete(Guid id);
}