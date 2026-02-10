using MySocialMedia.Api.Dtos;
using MySocialMedia.Api.Entities;

namespace MySocialMedia.Api.Services;

public interface IAuthorizeService
{
    Result<Token> Create(UserRegisterDto userRegisterDto);
}
