
using HolidayPlanner.DTOs;
using HolidayPlanner.Model;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authentication;

namespace HolidayPlanner.Services;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller {
	private readonly NpgsqlDataSource _dataSource;

	public UsersController(NpgsqlDataSource dataSource) {
		_dataSource = dataSource;
	}

	// Ryndee: I'm using \" because I got the error: relation "users" does not exist (SQLSTATE 42P01) when I tried to run SELECT * FROM Users; in the neon SQL Editor. I tried it in lowercase first, then went for the uppercase. I just checked and SELECT * FROM "users"; also gives that error, so it needs to be "Users".

	// Gets the Name and Username of a user from the Users table of the database
	[HttpGet("{id}")]
	public async Task<UserDto> GetUserById(string id) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "SELECT \"Id\", \"Name\", \"Username\" FROM \"Users\" WHERE \"Id\" = @id";
		command.Parameters.AddWithValue("id", id);

		var result = await command.ExecuteReaderAsync();
		var user = new List<UserDto>();

		while (await result.ReadAsync()) {
			user.Add(MapToDto(result));
		}

		return user[0];
	}

	// Gets the PasswordHash of the current user. Used when logging in
	public async Task<User> GetPasswordHashByUsername(string username) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();
	
		command.CommandText = "SELECT \"PasswordHash\" FROM \"Users\" WHERE \"Username\" = @username";
		command.Parameters.AddWithValue("username", username);
	
		var result = await command.ExecuteReaderAsync();
		var user = new List<User>();
	
		while (await result.ReadAsync()) {
			user[0].PasswordHash = result.GetString(result.GetOrdinal("PasswordHash"));
		}
	
		return user[0];
	}

	// Ryndee: CheckIfUsernameExists will be called before CreateUser and before the user is allowed to log in. Like this: 
	/* if(UserController.CheckIfUsernameExists(username) == false) {
		UserController.CreateUser(userDto); // Create user
		UserController.Login(password); // Login
	} */

	// Checks if the inputted username has an account
	public async Task<int?> CheckIfUsernameExists(string Username) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "SELECT \"Username\", \"Id\" FROM \"Users\" WHERE \"Username\" = @username";
		command.Parameters.AddWithValue("username", Username);

		var result = await command.ExecuteReaderAsync();
		var user = new List<UserDto>();

		while (await result.ReadAsync()) {
			user.Add(MapToDto(result));
		}

		return user[0].Id;
	}

	// Creates a new user to the Users table of the database
	public async Task<IActionResult> CreateUser(UserDto user, string password) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "INSERT INTO \"Users\" (\"Name\", \"Username\",\"PasswordHash\") VALUES (@name, @username, @password) RETURNING \"Id\"";
		command.Parameters.AddWithValue("id", user.Id);
		command.Parameters.AddWithValue("name", user.Name);
		command.Parameters.AddWithValue("username", user.Username);
		command.Parameters.AddWithValue("password", password);

		var newUserId = await command.ExecuteScalarAsync();    

		return CreatedAtAction(nameof(GetUserById), new { Id = newUserId });
	}

	// Changes the name and password of a user in the Users table of the database
	public async Task<IActionResult> UpdateUser(UserDto user, string password) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "UPDATE \"Users\" SET \"Name\" = @name, \"PasswordHash\" = @password WHERE \"Id\" = @id AND \"Username\" = @username";
		command.Parameters.AddWithValue("id", user.Id);
		command.Parameters.AddWithValue("name", user.Name);
		command.Parameters.AddWithValue("username", user.Username);
		command.Parameters.AddWithValue("password", password);

		var rowsAffected = await command.ExecuteNonQueryAsync();

		if (rowsAffected == 0) {
			return NotFound();
		}

		return NoContent();
	}

	// Deletes a user from the Users table of the database
  public async Task<IActionResult> DeleteUser(int id) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "DELETE FROM \"Users\" WHERE \"Id\" = @id";
		command.Parameters.AddWithValue("id", id);

		var rowsAffected = await command.ExecuteNonQueryAsync();

		if (rowsAffected == 0) {
			return NotFound();
		}

		return NoContent();
	}

	private static UserDto MapToDto(NpgsqlDataReader reader) => new() {
		Id       = reader.GetInt32(reader.GetOrdinal("Id")),
		Name     = reader.GetString(reader.GetOrdinal("Name")),
		Username = reader.GetString(reader.GetOrdinal("Username")),
	};

// 	public static string GenerateJwtToken() {
// 		var tokenHandler = new JwtSecurityTokenHandler();

// var tokenDescriptor = new SecurityTokenDescriptor
// {
//     Subject = new ClaimsIdentity(new[]
//     {
//         new Claim(ClaimTypes.Name, user.Username),
//         new Claim(ClaimTypes.Role, "Admin")
//     }),
//     Expires = DateTime.UtcNow.AddHours(1),
//     SigningCredentials = new SigningCredentials(
//         new SymmetricSecurityKey(key),
//         SecurityAlgorithms.HmacSha256Signature)
// };

// var token = tokenHandler.CreateToken(tokenDescriptor);

// return tokenHandler.WriteToken(token);
// 	}
}