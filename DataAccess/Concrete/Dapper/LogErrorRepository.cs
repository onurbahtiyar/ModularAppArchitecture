using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace DataAccess.Concrete.Dapper;

public class LogErrorRepository
{
    private readonly SqlConnection _connection;

    public LogErrorRepository(SqlConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public void LogError(Exception exception, object userInput = null, Guid? userGuid = null, string actionName = "", string controllerName = "")
    {
        try
        {
            string systemName = "LIVE";

#if DEVELOPER
            systemName = "DEVELOPER";
#endif

#if DEBUG
            systemName = "DEBUG";
#endif

            var errorLog = new
            {
                UserGuid = userGuid,
                ActionName = actionName,
                ControllerName = controllerName,
                Message = $"{exception.Message} | {exception.InnerException?.Message}",
                StackTrace = exception.StackTrace,
                UserInput = userInput != null ? JsonConvert.SerializeObject(userInput) : null,
                DateCreated = DateTime.Now,
                System = systemName
            };

            _connection.Open();
            var sql = @"INSERT INTO ErrorLog (UserGuid, ActionName, ControllerName, Message, StackTrace, UserInput, DateCreated, System)
                            VALUES (@UserGuid, @ActionName, @ControllerName, @Message, @StackTrace, @UserInput, @DateCreated, @System)";

            _connection.Execute(sql, errorLog);
        }
        catch (Exception)
        {
            return;
        }
        finally
        {
            _connection.Close();
        }
    }
}