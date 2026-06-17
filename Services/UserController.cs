
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

		string connectionString = ConfigurationHelper.GetConnectionString("Default");

		try {
			await using var dataSource = NpgsqlDataSource.Create(connectionString);
			await using var cmd = dataSource.CreateCommand("SELECT \"Id\", \"Name\", \"Username\" FROM \"Users\" WHERE \"Id\" = @id");

			using var reader = await cmd.ExecuteReaderAsync();
			var userDtos = new List<UserDto>();

			while (await reader.ReadAsync()) {
				var userId = reader.GetInt32(0);
				var name = reader.GetString(1);
				var username = reader.GetString(2);

				var dto = new UserDto { Id = userId, Name = name, Username = username };

				userDtos.Add(dto);
			}
		} catch (NpgsqlException ex) {
			Console.WriteLine($"Error: {ex.Message}");
		}


		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();
		int Id = Convert.ToInt32(id);

		command.CommandText = "SELECT \"Id\", \"Name\", \"Username\" FROM \"Users\" WHERE \"Id\" = @id";
		command.Parameters.AddWithValue("id", Id);

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

	// Checks if the inputted username has an account
	public async Task<UserDto>/*Task<bool>*/ CheckIfUsernameExists(string Username) {

		string connectionString = ConfigurationHelper.GetConnectionString("Default");

		// try {
			await using var dataSource = NpgsqlDataSource.Create(connectionString);
			await using var cmd = dataSource.CreateCommand("SELECT \"Username\" FROM \"Users\" WHERE \"Username\" = @username");

			cmd.Parameters.AddWithValue("@username", Username);

			using var reader = await cmd.ExecuteReaderAsync();
			var userDtos = new List<UserDto>();

			while (await reader.ReadAsync()) {
				var username = reader.GetString(0);

				var dto = new UserDto { Name = "name", Username = username };

				userDtos.Add(dto);
			}
			return userDtos[0];

		// } catch (NpgsqlException ex) {
		// 	Console.WriteLine($"Error: {ex.Message}");
		// }
		// return userDtos[0];

		// await using var connection = await _dataSource.OpenConnectionAsync();
		// await using var command = connection.CreateCommand();

		// command.CommandText = "SELECT \"Username\" FROM \"Users\" WHERE \"Username\" = @username";
		// command.Parameters.AddWithValue("username", Username);

		// var result = await command.ExecuteReaderAsync();
		// var user = new List<UserDto>();

		// while (await result.ReadAsync()) {
		// 	user.Add(MapToDto(result));
		// }

		// // if (user[0].Username == Username) {
		// // 	Console.WriteLine(value: $"usernames same? {user[0].Username} {Username}");
		// // 	return true;
		// // } else {
		// // 	Console.WriteLine(value: $"usernames same? {user[0].Username} {Username}");
		// // 	return false;
		// // }
		// return user[0];

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

		command.CommandText = "UPDATE \"Users\" SET \"Name\" = @name, \"PasswordHash\" = @password WHERE \"Id\" = @id";
		command.Parameters.AddWithValue("id", user.Id);
		command.Parameters.AddWithValue("name", user.Name);
		command.Parameters.AddWithValue("password", password);

		var rowsAffected = await command.ExecuteNonQueryAsync();

		if (rowsAffected == 0) {
			return NotFound();
		}

		return NoContent();
	}

	// Deletes a user from the Users table of the database
  public async Task<IActionResult> DeleteUser(UserDto user) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "DELETE FROM \"Users\" WHERE \"Id\" = @id AND \"Username\" = @username";
		command.Parameters.AddWithValue("id", user.Id);
		command.Parameters.AddWithValue("username", user.Username);

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

	// temporary get all users added by Olamilekan
	[HttpGet("GetAllUsers")]
	public async Task<List<UserDto>> GetAllUsers() {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "SELECT \"Id\", \"Name\", \"Username\" FROM \"Users\"";

		var result = await command.ExecuteReaderAsync();
		var users = new List<UserDto>();

		while (await result.ReadAsync()) {
			users.Add(MapToDto(result));
		}
		Console.WriteLine($"users: {users.Count}");
		users.ForEach(u => Console.WriteLine($"User: {u.Id}, {u.Name}, {u.Username}"));

		return users;
	}
	
}