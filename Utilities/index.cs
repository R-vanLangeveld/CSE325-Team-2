/* cse 340 js code but edited

const userModel = require("../Model/User");
const Util = {};
const jwt = require("jsonwebtoken");
require("dotenv").config();

Util.checkJWTToken = (req, res, next) => {
	if (req.cookies.jwt) {
		jwt.verify(
			req.cookies.jwt,
			process.env.ACCESS_TOKEN_SECRET,
			function (err, userData) {
				if (err) {
					req.flash("Please log in");
					res.clearCookie("jwt");
					res.locals.loggedin = 0;
					return res.redirect("/profile/login");
				}
				res.locals.userData = userData;
				res.locals.loggedin = 1;
				next();
			}
		);
	} else {
		next();
	}
}

Util.checkLogin = (req, res, next) => {
	if (res.locals.loggedin) {
		next();
	} else {
		req.flash("notice", "Please log in.");
		return res.redirect("/profile/login");
	}
}

Util.handleErrors = fn => (req, res, next) => Promise.resolve(fn(req, res, next)).catch(next);

module.exports = Util;
*/