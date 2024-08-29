using Dapper;
using Entities.Concrete.EntityFramework.Entities;
using System.Data;

namespace DataAccess.Concrete.Dapper;

public class MiddlewareRepository
{
    private readonly IDbConnection _connection;

    public MiddlewareRepository(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task AddUserActivityLogAsync(UserActivityLog userActivityLog)
    {
        try
        {
            var sql = @"INSERT INTO UserActivityLog (IPAddress, BrowserInfo, ActivityDate, ActivityType, ActivityDetail, UserGuid, AdditionalData, ActivityPage)
                                VALUES (@IPAddress, @BrowserInfo, @ActivityDate, @ActivityType, @ActivityDetail, @UserGuid, @AdditionalData, @ActivityPage)";
            await _connection.ExecuteAsync(sql, userActivityLog);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging user activity: {ex.Message}");
        }
    }
}
