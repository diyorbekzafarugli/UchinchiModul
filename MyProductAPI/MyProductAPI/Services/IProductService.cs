using MyProductAPI.Dtos;

namespace MyProductAPI.Services;

public interface IProductService
{
    Guid? Create(CreateProductDto dto);
    bool Update(UpdateProductDto dto);
    bool Delete(Guid id);
    ProductResultDto? GetById(Guid id);
    List<ProductResultDto> GetAll();
}