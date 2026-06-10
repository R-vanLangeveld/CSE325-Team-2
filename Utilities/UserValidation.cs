/* cse 340 js code but edited

const utilities = require(".");
const { body, validationResult } = require("express-validator");
const validate = {};
const userModel = require("../Model/User");

validate.registrationRules = () => {
	return [
		body("Name").trim().escape().notEmpty().isLength({ min: 1 }).withMessage("Please provide a name."),
		body("Username").trim().escape().notEmpty().isEmail().normalizeEmail().withMessage("A valid email is required.").custom(async (Username) => {
			const emailExists = await userModel.checkExistingEmail(Username);
			if (emailExists) {
				throw new Error("An account with that email already exists. Please log in or use a different email.");
			}
		}),
		body("PasswordHash").trim().notEmpty().isStrongPassword({ minLength: 12, minLowercase: 1, minUppercase: 1, minNumbers: 1, minSymbols: 1 }).withMessage("Password does not meet requirements.")
	]
}

validate.loginRules = () => {
	return [
		body("Username").trim().escape().notEmpty().isEmail().normalizeEmail().withMessage("A valid email is required.").custom(async (Username) => {
			const emailExists = await accountModel.checkExistingEmail(Username);
			if (!emailExists) {
				throw new Error("No account with that email exists. Please register or use a different email.");
			}
		}),
		body("PasswordHash").trim().notEmpty().isStrongPassword({ minLength: 12, minLowercase: 1, minUppercase: 1, minNumbers: 1, minSymbols: 1 }).withMessage("Password does not meet requirements.")
	]
}

validate.checkRegistrationData = async (req, res, next) => {
	const { Name, Username } = req.body;
	let errors = [];
	errors = validationResult(req);
	if (!errors.isEmpty()) {
		let nav = await utilities.getNav();
		res.render("account/registration", {errors, title: "Registration", nav, Name, Username});
		return
	}
	next();
}

validate.checkLoginData = async (req, res, next) => {
	const { Username } = req.body;
	let errors = [];
	errors = validationResult(req);
	if (!errors.isEmpty()) {
		let nav = await utilities.getNav();
		res.render("account/login", {errors, title: "Login", nav, Username});
		return
	}
	next();
}

module.exports = validate;
*/