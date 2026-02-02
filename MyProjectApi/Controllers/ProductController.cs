using Microsoft.AspNetCore.Mvc;
using MyProject.Dtos;
using MyProject.Interfaces;

namespace MyProjectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    // GetAll: api/products
    [HttpGet]
    public IActionResult GetAll()
    {
        var products = _service.GetAll();
        return Ok(ApiResponse<List<ProductResultDto>>.Ok(products));
    }

    // GetById: api/products/{id}
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var product = _service.GetById(id);

        if (product is null)
            return NotFound("Product topilmadi");

        return Ok(product);
    }

    // Create: api/products
    [HttpPost]
    public IActionResult Create(ProductCreateDto dto)
    {
        var id = _service.Add(dto);
        if (id is null)
            return BadRequest(ApiResponse<Guid>.Fail("Yaratishda xatolik"));

        return Ok(ApiResponse<Guid>.Ok(id.Value, "Maxsulot yaratildi"));
    }

    // Update: api/products
    [HttpPut]
    public IActionResult Update(ProductUpdateDto dto)
    {
        var updated = _service.Update(dto);

        if (!updated)
            return BadRequest("Update xato (Id yoki ma'lumot noto‘g‘ri)");

        return Ok(updated);
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var deleted = _service.Delete(id);

        if (!deleted)
            return NotFound("Product topilmadi");

        return Ok(deleted);
    }
}
