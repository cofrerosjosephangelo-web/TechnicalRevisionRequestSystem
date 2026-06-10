namespace TechnicalRevisionRequestSystem.Models
{
	public class UserModel
	{

		public string Id { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }

		public string PasswordHash { get; set; }

		public int DeptId { get; set; }
		public int SectionId { get; set; }

		public int safety { get; set; }
		public int jrsviewer { get; set; }
		public int ismntadmin { get; set; }
		public int jrsadmin { get; set; }

		public string email { get; set; }
		public int notifdownload { get; set; }

		public string userid { get; set; }
	}

	public class LoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
