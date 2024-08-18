namespace Business.Abstract;

public interface IErrorLogService
{
    public void LogError(Exception exception, object? userInput = null, string? UserGuid = null, string actionName = "", string controllerName = "");
}