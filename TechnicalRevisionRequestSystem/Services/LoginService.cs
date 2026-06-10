using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TechnicalRevisionRequestSystem.Models;



namespace TechnicalRevisionRequestSystem.Services
{
	public class LoginService
	{
		private readonly string _connectionString;

		public LoginService(IConfiguration config)
		{
			_connectionString = config.GetConnectionString("IdentityConnection");

		}

		public UserModel ValidateUser(string username, string password)
		{
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				string sql = "SELECT * FROM AspNetUsers WHERE UserName = @Username";

				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@Username", username);

				var reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					string storedHash = reader["PasswordHash"].ToString();

					//var hasher = new PasswordHasher<string>();
					//var result = hasher.VerifyHashedPassword(null, storedHash, password);

					var hasher = new PasswordHasher<string>();
					var result = hasher.VerifyHashedPassword(null, storedHash, password);

					if (result == PasswordVerificationResult.Success ||
	result == PasswordVerificationResult.SuccessRehashNeeded)
					{
						return new UserModel
						{
							Id = reader["Id"].ToString(),
							FullName = reader["FullName"].ToString(),
							UserName = reader["UserName"].ToString(),

							DeptId = Convert.ToInt32(reader["Dept_id"]),
							SectionId = Convert.ToInt32(reader["Section_id"]),

							safety = Convert.ToInt32(reader["safety"]),
							jrsviewer = Convert.ToInt32(reader["jrsviewer"]),
							ismntadmin = Convert.ToInt32(reader["ismntadmin"]),
							jrsadmin = Convert.ToInt32(reader["jrsadmin"]),

							email = reader["email"].ToString(),
							notifdownload = Convert.ToInt32(reader["notifdownload"]),

							userid = reader["id"].ToString()
						};
					}
				}
			}

			return null;
		}
	}
}
