using Microsoft.AspNetCore.Mvc;
using Order.Api.Common;
using Order.Api.Dtos;
using Order.Api.Services;

namespace Order.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public ActionResult<ApiResponse<Guid>> Create([FromBody] CreateOrderDto createDto)
    {
        var id = _orderService.Create(createDto);
        if (id == Guid.Empty) return BadRequest(ApiResponses.Fail<Guid>(
            "Validation error", new[] { "CustomerName >= 3, PhoneNumber >= 9, TotalAmount > 0 bo'lishi kerak" },
            HttpContext));
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponses.Ok(id, "Created", HttpContext));
    }

    [HttpGet]
    public ActionResult<ApiResponse<List<OrderResultDto>>> GetAll()
    {
        var orders = _orderService.GetAll();
        if (orders.Count == 0) return NotFound(ApiResponses.Fail<List<OrderResultDto>>(
            "Not found", new[] { "Ma'lumotlar topilmadi" }, HttpContext));
        return Ok(ApiResponses.Ok(orders, "Ok", HttpContext));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<OrderResultDto>> GetById(Guid id)
    {
        var result = _orderService.GetById(id);
        if (result is null) return NotFound(ApiResponses.Fail<OrderResultDto>(
            "Not found", new[] { "Order topilmadi" }, HttpContext));
        return Ok(ApiResponses.Ok(result, "Ok", HttpContext));
    }

    [HttpPatch("{id:guid}/status")]
    public ActionResult<ApiResponse<object>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
    {
        var ok = _orderService.UpdateStatus(id, updateOrderStatusDto.Status);

        if (!ok)
        {
            return BadRequest(ApiResponses.Fail<object>(
                "Status change not allowed", new[] { "Status o'tish qoidalariga to'g'ri kelmadi" },
                HttpContext));
        }
        return Ok(ApiResponses.Ok<object>("Status updated", HttpContext));
    }

    [HttpPut]
    public ActionResult<ApiResponse<object>> UpdateOrder(UpdateOrderDto updateOrderDto)
    {
        var order = _orderService.Update(updateOrderDto);
        if(!order) return NotFound(ApiResponses.Fail<object>(
            "Not found", new[] { "Order topilmadi" }, HttpContext));
        return Ok(ApiResponses.Ok<object>("Yangilandi", HttpContext));
    }

    [HttpDelete]
    public ActionResult<ApiResponse<object>> Delete(Guid id)
    {
        var order = _orderService.Delete(id);
        if (!order) return NotFound(ApiResponses.Fail<object>(
            "Not found", new[] { "Order topilmadi" }, HttpContext));
        return Ok(ApiResponses.Ok<object>("Yangilandi", HttpContext));
    }
}
