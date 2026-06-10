namespace HolidayPlanner.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        /* //cse 340 JS code but edited. Now its a mix of JS and C# but I don't if it should be in here or in another file

        static async Task<string> CheckExistingEmail(string Username) {
            try {
                const email = await pool.query("SELECT * FROM \"Users\" WHERE \"Username\" = $1", [Username]);
                return email.rowCount.ToString();
            } catch (Exception error) {
                return error.message;
            }
        }

        static async Task<string[]> UpdateUserInfo(int Id, string Name, string Username) {
            try {
                const user = await pool.query("UPDATE \"User\" SET \"Name\" = $1 \"PasswordHash\" = $2 WHERE \"Username\" = $3 AND \"Id\" = $4 RETURNING \"Name\", \"Username\" ", [Name, PasswordHash, Username, Id]);
                return user.rows[0];
            } catch (Exception error) {
                return error.message;
            }
        }

        module.exports = { registerUser, checkExistingEmail, getUserByEmail, updateUserInfo, updateUserPassword };

        // This is from the .txt files but edited to match That Stuff ^

        -- Get all Plans that a User is a collaborator on (if statement)
        static async Task<string[]> GetUserData(string Username, string columnName, string ASC_DESC) {
            if(columnName != "" && ASC_DESC != "") {
                try {
                    const userPlans = await pool.query("SELECT \"Plans.Name\", \"Description\", price, \"Date\", \"Collaborators\", \"Participants\", \"Creator\" FROM \"Users\" LEFT JOIN \"Plans\" ON \"Username\" = ANY(\"Collaborators\") WHERE \"Username\" = $1 ORDER BY $2 $3", [Username, columnName, ASC_DESC]);
                    return userPlans.rows;
                } catch (Exception error) {
                    return error.message;
                }
            } else {
                try {
                    const userPlans = await pool.query("SELECT \"Plans.Name\", \"Description\", price, \"Date\", \"Collaborators\", \"Participants\", \"Creator\" FROM \"Users\" LEFT JOIN \"Plans\" ON \"Username\" = ANY(\"Collaborators\") WHERE \"Username\" = $1", [Username]);
                    return userPlans.rows;
                } catch (Exception error) {
                    return error.message;
                }
            }
        }

        */
    }
}