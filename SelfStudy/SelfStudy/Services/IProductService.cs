using SelfStudy.Dtos;

namespace SelfStudy.Services;

public interface IProductService
{
    Guid? Create(ProductCreateDto dto);
    bool Update(ProductUpdateDto dto);
    bool Delete(Guid id);
    ProductResultDto Get(Guid id);
    List<ProductResultDto> GetAll();
    List<ProductResultDto> ProductsExspensiveThen(decimal minPrace);
    List<ProductResultDto> ProductsByName(string name);
}

