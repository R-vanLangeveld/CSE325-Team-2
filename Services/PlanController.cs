using CSE325_Team_2.DTOs;
using Npgsql;
using System.Data.Common;

namespace CSE325_Team_2.Services;

public class PlanService
{
    private readonly NpgsqlDataSource _dataSource;

    public PlanService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<List<PlanDto>> GetPlansByUserAsync(int userId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = @"SELECT DISTINCT p.*, 
                    u.""Id""       AS creator_id, 
                    u.""Name""     AS creator_name, 
                    u.""Username"" AS creator_username 
        FROM ""Plans"" p 
        INNER JOIN ""Users"" u ON p.""CreatorId"" = u.""Id"" 
        LEFT JOIN ""Users"" coll ON coll.""PlanId"" = p.""Id"" 
        WHERE p.""CreatorId"" = @userId OR coll.""Id"" = @userId";

        command.Parameters.AddWithValue("userId", userId);

        await using var result = await command.ExecuteReaderAsync();
        var plans = new List<PlanDto>();

        while (await result.ReadAsync())
        {
            plans.Add(MapToDto(result));
        }

        await result.CloseAsync();

        foreach (var plan in plans)
        {
            plan.Collaborators = await GetCollaboratorsByPlanIdAsync(plan.Id);
        }

        return plans;
    }

    public async Task<PlanDto?> GetPlanByIdAsync(int planId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = @"SELECT p.*, 
                    u.""Id"" as creator_id, 
                    u.""Name"" as creator_name, 
                    u.""Username"" as creator_username 
        FROM ""Plans"" p 
        INNER JOIN ""Users"" u ON p.""CreatorId"" = u.""Id"" 
        WHERE p.""Id"" = @planId";
        command.Parameters.AddWithValue("planId", planId);

        await using var result = await command.ExecuteReaderAsync();

        PlanDto? plan = null;
        if (await result.ReadAsync())
        {
            plan = MapToDto(result);
        }

        await result.CloseAsync();

        if (plan != null)
        {
            plan.Collaborators = await GetCollaboratorsByPlanIdAsync(plan.Id);
        }

        return plan;
    }

    public async Task<int> CreatePlanAsync(PlanDto planDto)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "INSERT INTO \"Plans\" (\"Name\", \"Description\", \"Price\", \"Date\", \"CreatorId\", \"Participants\") VALUES (@name, @description, @price, @date, @creatorId, @participants) RETURNING \"Id\"";
        command.Parameters.AddWithValue("name", planDto.Name);
        command.Parameters.AddWithValue("description", planDto.Description);
        command.Parameters.AddWithValue("price", planDto.Price);
        command.Parameters.AddWithValue("date", planDto.Date);
        command.Parameters.AddWithValue("creatorId", planDto.Creator.Id);
        command.Parameters.AddWithValue("participants", planDto.Participants ?? new List<string>());

        var newPlanId = await command.ExecuteScalarAsync();

        return (int)newPlanId!;
    }

    public async Task UpdatePlanAsync(int id, PlanDto planDto)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "UPDATE \"Plans\" SET \"Name\" = @name, \"Description\" = @description, \"Price\" = @price, \"Date\" = @date WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("name", planDto.Name);
        command.Parameters.AddWithValue("description", planDto.Description);
        command.Parameters.AddWithValue("price", planDto.Price);
        command.Parameters.AddWithValue("date", planDto.Date);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeletePlanAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM \"Plans\" WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }

    
    public async Task<bool> AddCollaboratorAsync(int id, string username)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        
        await using var findCommand = connection.CreateCommand();
        findCommand.CommandText = "SELECT \"Id\" FROM \"Users\" WHERE \"Username\" = @username";
        findCommand.Parameters.AddWithValue("username", username);
        
        var userId = await findCommand.ExecuteScalarAsync();
        
        if (userId == null)
        {
            return false; 
        }
        
        await using var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = "UPDATE \"Users\" SET \"PlanId\" = @planId WHERE \"Id\" = @userId";
        updateCommand.Parameters.AddWithValue("planId", id);
        updateCommand.Parameters.AddWithValue("userId", (int)userId);

        await updateCommand.ExecuteNonQueryAsync();
        return true; 
    }

    public async Task RemoveCollaboratorAsync(int id, int userId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "UPDATE \"Users\" SET \"PlanId\" = NULL WHERE \"Id\" = @userId AND \"PlanId\" = @planId";
        command.Parameters.AddWithValue("planId", id);
        command.Parameters.AddWithValue("userId", userId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task AddInviteeAsync(int id, string invitee)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "UPDATE \"Plans\" SET \"Participants\" = array_append(\"Participants\", @name) WHERE \"Id\" = @planId";
        command.Parameters.AddWithValue("planId", id);
        command.Parameters.AddWithValue("name", invitee);

        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveInviteeAsync(int id, string invitee)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "UPDATE \"Plans\" SET \"Participants\" = array_remove(\"Participants\", @name) WHERE \"Id\" = @planId";
        command.Parameters.AddWithValue("planId", id);
        command.Parameters.AddWithValue("name", invitee);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<UserDto>> GetCollaboratorsByPlanIdAsync(int planId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "SELECT \"Id\", \"Name\", \"Username\" FROM \"Users\" WHERE \"PlanId\" = @planId";
        command.Parameters.AddWithValue("planId", planId);

        var collaborators = new List<UserDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            collaborators.Add(new UserDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Username = reader.GetString(reader.GetOrdinal("Username"))
            });
        }
        return collaborators;
    }

    private static PlanDto MapToDto(NpgsqlDataReader reader) => new()
    {
        Id          = reader.GetInt32(reader.GetOrdinal("Id")),
        Name        = reader.GetString(reader.GetOrdinal("Name")),
        Description = reader.GetString(reader.GetOrdinal("Description")),
        Price       = reader.GetDecimal(reader.GetOrdinal("Price")),
        Date        = reader.GetDateTime(reader.GetOrdinal("Date")),
        Participants = reader.GetFieldValue<List<string>>(reader.GetOrdinal("Participants")),
        Creator = new UserDto
        {
            Id       = reader.GetInt32(reader.GetOrdinal("creator_id")),
            Name     = reader.GetString(reader.GetOrdinal("creator_name")),
            Username = reader.GetString(reader.GetOrdinal("creator_username")),
        }
    };
}