using Microsoft.AspNetCore.Mvc;
using MyProductAPI.Dtos;
using MyProductAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProductAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/<ProductsController>
    [HttpGet("get-all")]
    public List<ProductResultDto> Get()
    {
        return _productService.GetAll();
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public ProductResultDto? Get(Guid id)
    {
        return _productService.GetById(id);
    }

    // POST api/<ProductsController>
    [HttpPost]
    public ActionResult<Guid> Post(CreateProductDto createProduct)
    {
        var id = _productService.Create(createProduct);
        if (id is null) return BadRequest("Product ma'lumotlari noto'g'ri");
        return Ok(id.Value);
    }

    // PUT api/<ProductsController>/5   
    [HttpPut("update")]
    public bool Put(Guid id, [FromBody] UpdateProductDto updateProduct)
    {
        return _productService.Update(updateProduct);
    }

    // DELETE api/<ProductsController>/5
    [HttpDelete("delete")]
    public bool Delete(Guid id)
    {
        return _productService.Delete(id);
    }
}
