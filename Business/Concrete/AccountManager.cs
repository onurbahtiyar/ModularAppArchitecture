using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using Enigma;
using Entities.Concrete.EntityFramework.Entities;
using Entities.Dtos;
using System.Security.Cryptography;

namespace Business.Concrete;

public class AccountManager : IAccountService
{
    private readonly IUserDal userDal;
    private readonly IErrorLogService errorLogService;
    private readonly Processor processor;
    private readonly ITokenHelper tokenHelper;
    private readonly IRoleDal roleDal;
    private readonly IUserRoleDal userRoleDal;

    public AccountManager(IUserDal userDal, IErrorLogService errorLogService, Processor processor, ITokenHelper tokenHelper, IRoleDal roleDal, IUserRoleDal userRoleDal)
    {
        this.userDal = userDal;
        this.errorLogService = errorLogService;
        this.processor = processor;
        this.tokenHelper = tokenHelper;
        this.roleDal = roleDal;
        this.userRoleDal = userRoleDal;
    }

    public IDataResult<AccessToken> Login(LoginDto loginDto)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                loginDto.Password = processor.EncryptorSymmetric(loginDto.Password, aes);
            }

            var user = userDal.Get(
                    x => (x.Username == loginDto.Username || x.Email == loginDto.Username) && x.Password == loginDto.Password,
                    u => u.UserRoles
                );
            if (user == null)
            {
                return new ErrorDataResult<AccessToken>(data: null, SystemMessages.InvalidCredentials);
            }

            var userRoles = userRoleDal.GetList(x => x.UserGuid == user.Guid).Select(x=> x.Role.RoleName).ToList();

            var token = tokenHelper.CreateToken(new TokenUser
            {
                UserId = user.Id,
                Guid = user.Guid,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles
            });

            return new SuccessDataResult<AccessToken>(token, SystemMessages.OperationSuccessful);
        }
        catch (Exception ex)
        {
            errorLogService.LogError(ex, new { loginDto = loginDto }, null, "Login", "AccountManager");
            return new ErrorDataResult<AccessToken>(null, SystemMessages.UnknownError);
        }
    }

    public IDataResult<AccessToken> Register(RegisterDto registerDto)
    {
        try
        {
            if (userDal.GetList(x => x.Email == registerDto.Email).Any())
            {
                return new ErrorDataResult<AccessToken>(null, SystemMessages.ResourceAlreadyExists);
            }

            using (Aes aes = Aes.Create())
            {
                registerDto.Password = processor.EncryptorSymmetric(registerDto.Password, aes);
            }

            var user = new User
            {
                Guid = Guid.NewGuid(),
                Email = registerDto.Email,
                Password = registerDto.Password,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Username = registerDto.Username,
                PhoneNumber = registerDto.PhoneNumber,
                DateOfBirth = registerDto.DateOfBirth,
                ProfilePictureUrl = registerDto.ProfilePictureUrl,
                Gender = registerDto.Gender,
                Address = registerDto.Address,
                Country = registerDto.Country,
                PreferredLanguage = registerDto.PreferredLanguage,
                TwoFactorEnabled = registerDto.TwoFactorEnabled,
                CreatedDate = DateTime.Now
            };
            userDal.Add(user);

            var userRole = new UserRole
            {
                UserGuid = user.Guid,
                RoleId = roleDal.Get(x => x.RoleName == "User").RoleId,
                AssignedDate = DateTime.Now
            };

            userRoleDal.Add(userRole);

            user = userDal.Get(
                x => x.Email == registerDto.Email,
                u => user.UserRoles
            );

            var token = tokenHelper.CreateToken(new TokenUser
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoleDal.GetList(x => x.UserGuid == user.Guid)
                   .Select(x => x.Role.RoleName)
                   .ToList()
            });

            return new SuccessDataResult<AccessToken>(token, SystemMessages.OperationSuccessful);
        }
        catch (Exception ex)
        {
            errorLogService.LogError(ex, new { registerDto = registerDto }, null, "Register", "AccountManager");
            return new ErrorDataResult<AccessToken>(null, SystemMessages.UnknownError);
        }
    }
}