using MyProject.Dtos;

namespace MyProject.Interfaces;

public interface IProductService
{
    Guid? Add(ProductCreateDto productDto);
    List<ProductResultDto> GetAll();
    ProductResultDto? GetById(Guid id);
    bool Update(ProductUpdateDto productUpdateDto);
    bool Delete(Guid id);
}
