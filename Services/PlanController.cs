
using HolidayPlanner.DTOs;
using HolidayPlanner.Model;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace HolidayPlanner.Services;

[ApiController]
[Route("api/[controller]")]
public class PlansController : Controller
{
    private readonly NpgsqlDataSource _dataSource;

    public PlansController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetPlansByUser(string userId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "SELECT p.*, u.id as creator_id, u.name as creator_name, u.email as creator_email FROM \"Plans\" p INNER JOIN \"Users\" u ON p.\"CreatorId\" = u.\"Id\" WHERE p.\"Id\" = @id";
        command.Parameters.AddWithValue("id", userId);

        var result = await command.ExecuteReaderAsync();

        var plans = new List<PlanDto>();

        while (await result.ReadAsync())
        {
            plans.Add(MapToDto(result));
        }
        
        return Ok(plans);
        
    }

    public async Task<IActionResult> CreatePlan(PlanDto planDto)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "INSERT INTO \"Plans\" (\"Name\", \"Description\", \"Price\", \"Date\", \"CreatorId\") VALUES (@name, @description, @price, @date, @creatorId) RETURNING \"Id\"";
        command.Parameters.AddWithValue("name", planDto.Name);
        command.Parameters.AddWithValue("description", planDto.Description);
        command.Parameters.AddWithValue("price", planDto.Price);
        command.Parameters.AddWithValue("date", planDto.Date);
        command.Parameters.AddWithValue("creatorId", planDto.Creator.Id);

        var newPlanId = await command.ExecuteScalarAsync();    

        return CreatedAtAction(nameof(GetPlansByUser), new { userId = planDto.Creator.Id }, new { Id = newPlanId });
    }

    public async Task<IActionResult> UpdatePlan(PlanDto planDto)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "UPDATE \"Plans\" SET \"Name\" = @name, \"Description\" = @description, \"Price\" = @price, \"Date\" = @date WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("id", planDto.Id);
        command.Parameters.AddWithValue("name", planDto.Name);
        command.Parameters.AddWithValue("description", planDto.Description);
        command.Parameters.AddWithValue("price", planDto.Price);
        command.Parameters.AddWithValue("date", planDto.Date);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
            return NotFound();

        return NoContent();
    }

    public async Task<IActionResult> DeletePlan(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM \"Plans\" WHERE \"Id\" = @id";
        command.Parameters.AddWithValue("id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
            return NotFound();

        return NoContent();
    }

    private static PlanDto MapToDto(NpgsqlDataReader reader) => new()
    {
        Id          = reader.GetInt32(reader.GetOrdinal("id")),
        Name        = reader.GetString(reader.GetOrdinal("name")),
        Description = reader.GetString(reader.GetOrdinal("description")),
        Price       = reader.GetDecimal(reader.GetOrdinal("price")),
        Date        = reader.GetDateTime(reader.GetOrdinal("date")),
        Creator = new UserDto
        {
            Id       = reader.GetInt32(reader.GetOrdinal("creator_id")),
            Name     = reader.GetString(reader.GetOrdinal("creator_name")),
            Username = reader.GetString(reader.GetOrdinal("creator_username")),
        }
    };
}