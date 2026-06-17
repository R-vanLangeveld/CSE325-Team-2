using Npgsql;
using CSE325_Team_2.DTOs;

namespace CSE325_Team_2.Services;

public class EventTaskService
{
    private readonly NpgsqlDataSource _dataSource;

    public EventTaskService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<List<EventTaskDto>> GetTasksByPlanIdAsync(int planId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = @"SELECT et.""Id"", et.""Name"", et.""Description"", et.""IsCompleted"", u.""Name"" AS assignee_name 
            FROM ""EventTasks"" et 
            LEFT JOIN ""Users"" u ON et.""AssigneeId"" = u.""Id"" 
            WHERE et.""PlanId"" = @planId";
        command.Parameters.AddWithValue("@planId", planId);

        var tasks = new List<EventTaskDto>();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(new EventTaskDto {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                IsCompleted = reader.GetBoolean(3),
                Assignee = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return tasks; 
    }   

    public async Task<int> AddTaskAsync(EventTaskDto task)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        
        int? assigneeId = null;
        if (!string.IsNullOrEmpty(task.Assignee))
        {
            await using var findCommand = connection.CreateCommand();
            findCommand.CommandText = "SELECT \"Id\" FROM \"Users\" WHERE \"Username\" = @assignee OR \"Name\" = @assignee LIMIT 1";
            findCommand.Parameters.AddWithValue("assignee", task.Assignee);
            var result = await findCommand.ExecuteScalarAsync();
            if (result != null)
            {
                assigneeId = (int)result;
            }
        }

        // Fallback to plan creator if assigneeId is still null
        if (assigneeId == null)
        {
            await using var creatorCommand = connection.CreateCommand();
            creatorCommand.CommandText = "SELECT \"CreatorId\" FROM \"Plans\" WHERE \"Id\" = @planId";
            creatorCommand.Parameters.AddWithValue("planId", task.PlanId);
            var result = await creatorCommand.ExecuteScalarAsync();
            if (result != null)
            {
                assigneeId = (int)result;
            }
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO \"EventTasks\" (\"Name\", \"Description\", \"IsCompleted\", \"AssigneeId\", \"PlanId\") VALUES (@name, @description, @isCompleted, @assigneeId, @planId) RETURNING \"Id\"";
        command.Parameters.AddWithValue("@name", task.Name);
        command.Parameters.AddWithValue("@description", task.Description);
        command.Parameters.AddWithValue("@isCompleted", task.IsCompleted);
        command.Parameters.AddWithValue("@assigneeId", assigneeId!.Value);
        command.Parameters.AddWithValue("@planId", task.PlanId); 

        var newTaskId = (int)await command.ExecuteScalarAsync()!;
        return newTaskId; 
    }    

    public async Task UpdateTaskAsync(EventTaskDto task)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        
        int? assigneeId = null;
        if (!string.IsNullOrEmpty(task.Assignee))
        {
            await using var findCommand = connection.CreateCommand();
            findCommand.CommandText = "SELECT \"Id\" FROM \"Users\" WHERE \"Username\" = @assignee OR \"Name\" = @assignee LIMIT 1";
            findCommand.Parameters.AddWithValue("assignee", task.Assignee);
            var result = await findCommand.ExecuteScalarAsync();
            if (result != null)
            {
                assigneeId = (int)result;
            }
        }

        // Fallback to plan creator if assigneeId is still null
        if (assigneeId == null)
        {
            await using var creatorCommand = connection.CreateCommand();
            creatorCommand.CommandText = "SELECT p.\"CreatorId\" FROM \"EventTasks\" et INNER JOIN \"Plans\" p ON et.\"PlanId\" = p.\"Id\" WHERE et.\"Id\" = @taskId";
            creatorCommand.Parameters.AddWithValue("taskId", task.Id);
            var result = await creatorCommand.ExecuteScalarAsync();
            if (result != null)
            {
                assigneeId = (int)result;
            }
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE \"EventTasks\" SET \"Name\" = @name, \"Description\" = @description, \"IsCompleted\" = @isCompleted, \"AssigneeId\" = @assigneeId WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("@id", task.Id);
        command.Parameters.AddWithValue("@name", task.Name);
        command.Parameters.AddWithValue("@description", task.Description);
        command.Parameters.AddWithValue("@isCompleted", task.IsCompleted);
        command.Parameters.AddWithValue("@assigneeId", assigneeId!.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task ToggleTaskAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "UPDATE \"EventTasks\" SET \"IsCompleted\" = NOT \"IsCompleted\" WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM \"EventTasks\" WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }
}