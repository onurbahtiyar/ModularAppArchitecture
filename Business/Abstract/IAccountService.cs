using Core.Utilities.Result.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Dtos;

namespace Business.Abstract;

public interface IAccountService
{
    IDataResult<AccessToken> Login(LoginDto loginDto);
    IDataResult<AccessToken> Register(RegisterDto registerDto);
}
