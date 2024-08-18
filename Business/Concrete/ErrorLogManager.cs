using Business.Abstract;
using Entities.Concrete.EntityFramework.Entities;
using Newtonsoft.Json;

namespace Business.Concrete;

public class ErrorLogManager : IErrorLogService
{
    private readonly IErrorLogDal _IErrorLogsDal;

    public ErrorLogManager(IErrorLogDal iErrorLogsDal)
    {
        _IErrorLogsDal = iErrorLogsDal;
    }

    public void LogError(Exception exception, object userInput = null, string? UserGuid = null, string actionName = "", string controllerName = "")
    {
        try
        {
#if DEVELOPER
            var errorLogDeveloper = new ErrorLog
            {
                UserGuid = Guid.TryParse(UserGuid, out var parsedGuidDeveloper) ? parsedGuidDeveloper : Guid.Empty,
                ActionName = actionName,
                ControllerName = controllerName,
                Message = $"{exception.Message} | {exception.InnerException}",
                StackTrace = exception.StackTrace,
                UserInput = userInput != null ? JsonConvert.SerializeObject(userInput) : null,
                DateCreated = DateTime.Now,
                System = "DEVELOPER"
            };

            _IErrorLogsDal.Add(errorLogDeveloper);
            return;
#endif

            var errorLog = new ErrorLog
            {
                UserGuid = Guid.TryParse(UserGuid, out var parsedGuid) ? parsedGuid : Guid.Empty,
                ActionName = actionName,
                ControllerName = controllerName,
                Message = $"{exception.Message} | {exception.InnerException}",
                StackTrace = exception.StackTrace,
                UserInput = userInput != null ? JsonConvert.SerializeObject(userInput) : null,
                DateCreated = DateTime.Now,
                System = "LIVE"
            };

            _IErrorLogsDal.Add(errorLog);
        }
        catch (Exception)
        {
            return;
        }
    }
}
