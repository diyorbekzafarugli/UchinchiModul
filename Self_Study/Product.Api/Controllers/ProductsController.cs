using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Dtos;
using Product.Api.Services;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public ActionResult<Guid> Create(CreateProductDto createProductDto)
    {
        var id = _productService.Create(createProductDto);
        if (id == Guid.Empty) return BadRequest("Yaratib bo'lmadi (validatsiya xatoligi)");
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ResultProductDto> GetById(Guid id)
    {
        var product = _productService.GetById(id);
        if (product is null) return NotFound("Product topilmadi");
        return Ok(product);
    }

    [HttpGet]
    public ActionResult<List<ResultProductDto>> GetAll()
    {
        var products = _productService.GetAll();
        if (products.Count == 0) return NotFound("Hali maxsulot qo'lshilmagan");
        return Ok(products);
    }

    [HttpPatch]
    public IActionResult Update(Guid id, UpdateProductDto updateProductDto)
    {
        if (id != updateProductDto.Id) return BadRequest("ID lar mos emas");
        var result = _productService.Update(id, updateProductDto);

        if (result == false) return NotFound();
        return NoContent();
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("ID xato");
        bool result = _productService.Delete(id);

        if (!result) return NotFound();
        return NoContent();
    }
}
