
using HolidayPlanner.DTOs;
using HolidayPlanner.Model;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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
	public async Task<IActionResult> GetUserById(string id) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "SELECT \"Id\", \"Name\", \"Username\" FROM \"Users\" WHERE \"Id\" = @id";
		command.Parameters.AddWithValue("id", id);

		var result = await command.ExecuteReaderAsync();
		var user = new List<UserDto>();

		while (await result.ReadAsync()) {
			user.Add(MapToDto(result));
		}

		return Ok(user);
	}

	// Ryndee: Used when logging in. maybe, I don't know
	/*
	public async Task<IActionResult> GetUserByUsername(string username) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "SELECT \"Name\", \"Username\" FROM \"Users\" WHERE \"Username\" = @username";
		command.Parameters.AddWithValue("username", username);

		var result = await command.ExecuteReaderAsync();
		var user = new List<UserDto>();

		while (await result.ReadAsync()) {
			user.Add(MapToDto(result));
		}

		return Ok(user);
	}
  */

	// Ryndee: CheckIfUsernameExists will be called before CreateUser and before the user is allowed to log in. Like this: 
	/* if(UserController.CheckIfUsernameExists(username) == false) {
		UserController.CreateUser(userDto); // Create user
		UserController.Login(password); // Login
	} */

	// Checks if the inputted username has an account
	public async Task<bool> CheckIfUsernameExists(string Username) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "SELECT \"Username\" FROM \"Users\" WHERE \"Username\" = @username";
		command.Parameters.AddWithValue("username", Username);

		var result = await command.ExecuteReaderAsync();
		var user = new List<UserDto>();

		while (await result.ReadAsync()) {
			user.Add(MapToDto(result));
		}

		return user.Count > 0;
	}

	// Ryndee: I don't fully understand DTO's, but we need to create the password somewhere. And it would be nice if the user could change it, but only if they're logged in.
	/* string passwordHash = new PasswordHasher<User>().HashPassword(user, password);
	var hashVarificationResult = new  PasswordHasher<User>().VerifyHashedPassword(null, passwordHash, password);
	switch (hashVarificationResult) {
		case hashVarificationResult.Failed:
			Console.WriteLine("Incorrect Password"); // Login
			Console.WriteLine("Password failed to properly hash"); // Create user
			break;
		case hashVarificationResult.Success:
			Console.WriteLine("The password is correct"); // Login
			Console.WriteLine("Password successfully hashed"); // Create user
			break;
		case hashVarificationResult.SuccessRehashNeeded:
			Console.WriteLine("Password ok but should be rehashed and updated.");
			break;
		
		default:
			throw new ArgumentOutOfRangeException();
		} */
	// Ryndee: Thank you Pang on Stack Overflow for the example of how to use the PasswordHasher (https://stackoverflow.com/questions/4181198/how-to-hash-a-password about halfway down the page)

	// Creates a new user to the Users table of the database
	public async Task<IActionResult> CreateUser(UserDto userDto) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "INSERT INTO \"Users\" (\"Name\", \"Username\") VALUES (@name, @username) RETURNING \"Id\"";
		command.Parameters.AddWithValue("id", userDto.Id);
		command.Parameters.AddWithValue("name", userDto.Name);
		command.Parameters.AddWithValue("username", userDto.Username);

		var newUserId = await command.ExecuteScalarAsync();    

		return CreatedAtAction(nameof(GetUserById), new { Id = newUserId });
	}

	// Changes the name and password of a user in the Users table of the database
	public async Task<IActionResult> UpdateUser(UserDto userDto) {
		await using var connection = await _dataSource.OpenConnectionAsync();
		await using var command = connection.CreateCommand();

		command.CommandText = "UPDATE \"Users\" SET \"Name\" = @name, \"Username\" = @username WHERE \"Id\" = @id";
		command.Parameters.AddWithValue("id", userDto.Id);
		command.Parameters.AddWithValue("name", userDto.Name);
		command.Parameters.AddWithValue("username", userDto.Username);

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
}


/* cse 340 js code but edited

async function registerUser(req, res) {
	let nav = await utilities.getNav();
	const { Name, Username, PasswordHash } = req.body;
	
	let hashedPassword;
	try {
		hashedPassword = await bcrypt.hashSync(PasswordHash, 10);
	} catch (error) {
		req.flash("notice", "Sorry, there was an error processing the registration.");
		res.status(500).render("profile/registration", {title: "Register", nav, errors: null});
	}

	const regResult = await userModel.registerUser(
		Name,
		Username,
		hashedPassword
	);

	if (regResult) {
		req.flash("notice", `Thank you ${Name} for registering. Please log in.`);
		res.status(201).render("profile/login", {title: "Login", nav, errors: null});
	} else {
		req.flash("notice", "Sorry, the registration failed.")
		res.status(501).render("profile/registration", {title: "Register", nav});
	}
}

async function userLogin(req, res) {
	let nav = await utilities.getNav();
	const { Username, PasswordHash } = req.body;
	const userData = await userModel.getUserByEmail(Username);
	if (!userData) {
		req.flash("notice", "Please check your credentials and try again.");
		res.status(400).render("profile/login", {title: "Login", nav, errors: null, Username});
		return
	}
	try {
		if (await bcrypt.compare(PasswordHash, userData.PasswordHash)) {
			delete userData.PasswordHash;
			const accessToken = jwt.sign(userData, process.env.ACCESS_TOKEN_SECRET, {expiresIn: 3600 * 1000 });
			if (process.env.NODE_ENV === "development") {
				res.cookie("jwt", accessToken, { httpOnly: true, maxAge: 3600 * 1000 });
			} else {
				res.cookie("jwt", accessToken, { httpOnly: true, secure: true, maxAge: 3600 * 1000 });
			}
			return res.redirect("/profile/");
		} else {
			req.flash("message notice", "Please check your credentials and try again.");
			res.status(400).render("profile/login", {title: "Login", nav, errors: null, Username});
		}
	} catch (error) {
		throw new Error("Access Forbidden");
	}
}

async function buildUpdateUser(req, res, next) {
	let nav = await utilities.getNav();
	
  const Id = parseInt(req.params.Id);
	const userInfo = await userModel.getUserById(Id);
	
	res.render("user/update", {title: "Edit Profile", nav, errors: null, Id: userInfo[0].Id, Name: userInfo[0].Name});
}

async function updateUserInfo(req, res) {
	let nav = await utilities.getNav();
	const { Id, Name, PasswordHash } = req.body;
	
	const updateResult = await userModel.updateUserInfo(
		Id, 
		Name,
		PasswordHash
	);

	const userData = await userModel.getUserById(Id);
	if (userData) {
		try {
			delete userData.PasswordHash;
			const accessToken = jwt.sign(userData, process.env.ACCESS_TOKEN_SECRET, {expiresIn: 3600 * 1000 });
			if (process.env.NODE_ENV === "development") {
				res.cookie("jwt", accessToken, { httpOnly: true, maxAge: 3600 * 1000 });
			} else {
				res.cookie("jwt", accessToken, { httpOnly: true, secure: true, maxAge: 3600 * 1000 });
			}
		} catch (error) {
			console.log(error);
		}
		req.flash("notice", "Your Profile has been updated.");
		res.render("profile", {title: "Profile", nav, errors: null});
	} else {
		req.flash("notice", "Sorry, the update failed.")
		res.status(501).render("profile/update", {title: "Edit Profile", nav});
	}
}

*/