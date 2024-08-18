using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace DataAccess.Concrete.Dapper;

public class GenericRepository
{
    private readonly SqlConnection connection;

    public GenericRepository(SqlConnection connection)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public IDataResult<T> Get<T>(string sql, object parameters = null)
    {
        try
        {
            connection.Open();
            var result = connection.QueryFirstOrDefault<T>(sql, parameters);
            return new SuccessDataResult<T>(result);
        }
        catch (Exception ex)
        {
            LogError(ex, new { sql, parameters }, "DAPPER", "Get", "GenericRepository");
            return new ErrorDataResult<T>(default(T), ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public IDataResult<List<T>> GetList<T>(string sql, object parameters = null)
    {
        try
        {
            connection.Open();
            var resultList = connection.Query<T>(sql, parameters).ToList();
            return new SuccessDataResult<List<T>>(resultList);
        }
        catch (Exception ex)
        {
            LogError(ex, new { sql, parameters }, "DAPPER", "GetList", "GenericRepository");
            return new ErrorDataResult<List<T>>(null, ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public IDataResult<bool> Update(string sql, object parameters)
    {
        try
        {
            connection.Open();
            var affectedRows = connection.Execute(sql, parameters);
            return new SuccessDataResult<bool>(affectedRows > 0);
        }
        catch (Exception ex)
        {
            LogError(ex, parameters, "Update", sql);
            return new ErrorDataResult<bool>(false, ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public IDataResult<bool> Delete(string sql, object parameters)
    {
        try
        {
            connection.Open();
            var affectedRows = connection.Execute(sql, parameters);
            return new SuccessDataResult<bool>(affectedRows > 0);
        }
        catch (Exception ex)
        {
            LogError(ex, parameters, "Delete", sql);
            return new ErrorDataResult<bool>(false, ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public IDataResult<List<T>> ExecuteStoredProcedure<T>(string storedProcedure, object parameters)
    {
        try
        {
            connection.Open();
            var resultList = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();
            return new SuccessDataResult<List<T>>(resultList);
        }
        catch (Exception ex)
        {
            LogError(ex, parameters, "ExecuteStoredProcedure", storedProcedure);
            return new ErrorDataResult<List<T>>(null, ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public void LogError(Exception exception, object userInput = null, string? userName = null, string actionName = "", string controllerName = "")
    {
        try
        {
            string systemName = "LIVE";

#if DEVELOPER
            systemName = "DEVELOPER";
#endif

#if DEBUG
                    systemName = "TEST";
#endif

            var errorLog = new
            {
                Username = userName,
                ActionName = actionName,
                ControllerName = controllerName,
                Message = $"{exception.Message} | {exception.InnerException?.Message}",
                StackTrace = exception.StackTrace,
                UserInput = userInput != null ? JsonConvert.SerializeObject(userInput) : null,
                DateCreated = DateTime.Now,
                System = systemName
            };

            connection.Open();
            var sql = @"INSERT INTO ErrorLogs (Username, ActionName, ControllerName, Message, StackTrace, UserInput, DateCreated, System)
                            VALUES (@Username, @ActionName, @ControllerName, @Message, @StackTrace, @UserInput, @DateCreated, @System)";

            connection.Execute(sql, errorLog);
        }
        catch (Exception)
        {
            return;
        }
    }
}