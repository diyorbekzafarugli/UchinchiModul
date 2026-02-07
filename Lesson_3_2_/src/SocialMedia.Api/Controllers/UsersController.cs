using Microsoft.AspNetCore.Mvc;
using SocialMedia.Api.Dtos;
using SocialMedia.Api.Services;

namespace SocialMedia.Api.Controllers;

[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost]
    public ActionResult<Guid> CreateUser([FromBody] UserCreateDto dto)
    {
        Guid? id = _userService.Create(dto);

        if (!id.HasValue || id.Value == Guid.Empty)
            return BadRequest("Foydalanuvchi yaratilmadi");

        return Ok(id.Value);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<UserGetDto> GetUser(Guid id)
    {
        var result = _userService.GetUser(id);
        if (result is null) return NotFound("Foydalanuvchi topilmadi");
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<List<UserGetDto>> GetAllUsers()
    {
        var result = _userService.GetUsers();
        if (result.Count == 0) return NotFound("Ro'yxatdan o'tgan foydalanuchilar topilmadi");
        return Ok(result);
    }

    [HttpPatch]
    public IActionResult UpdateUser(Guid id, string oldPassword, UserUpdateDto user)
    {
        var result = _userService.Update(id, oldPassword, user);
        if (!result) return NotFound("foydalanuvchi topilmadi");
        return Ok("Ma'ulmotlar yangilandi");
    }

    [HttpDelete]
    public IActionResult DeleteUser(Guid id)
    {
        var result = _userService.Delete(id);
        if (!result) return NotFound("ma'lumot xato kiritildi yoki foydalnuvchi ro'ycatdan o'tmagan");
        return Ok("Foydalanuvchi o'chirildi");
    }

    [HttpPost("post")]
    public ActionResult<Guid> AddPost(Guid userId, PostAddDto postAddDto)
    {
        var result = _userService.AddPost(userId, postAddDto);
        if (result == Guid.Empty) return NotFound("Ma'lumotlar noto'g'ri kiritildi. Qaytadan urib ko'ring");
        return Ok(result);
    }

    [HttpGet("{postId:guid}/post")]
    public ActionResult<PostGetDto> GetPost(Guid userId, string userName)
    {
        var result = _userService.GetPost(userId, userName);
        if (result == null) return NotFound("Topilmadi");
        return Ok(result);
    }

    [HttpPut("post-edit")]
    public IActionResult EditePost(Guid userId, Guid postId, string title, string content)
    {
        bool ok = _userService.EditPost(userId, postId, title, content);
        if (!ok) return BadRequest("Ma'lumotlar xato kiritildi..");
        return Ok("Post yangilandi");
    }

    [HttpGet("get-all-posts")]
    public ActionResult<PostGetDto> GetAllPosts()
    {
        var result = _userService.GetAllPosts();
        if (result.Count == 0) return NotFound("Postlar topilmadi");
        return Ok(result);
    }

    [HttpDelete("post")]
    public IActionResult DeletePost(Guid userId, Guid postId)
    {
        bool ok = _userService.DeletePost(userId, postId);
        if (!ok) return NotFound("Topilmadi");
        return Ok("post o'chirildi");
    }
}
