using Microsoft.AspNetCore.Mvc;
using TechnicalRevisionRequestSystem.Services;

namespace TechnicalRevisionRequestSystem.Controllers.Auth
{
	public class AuthController : Controller
	{
		private readonly LoginService _loginService;

		public AuthController(LoginService loginService)
		{
			_loginService = loginService;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}


		[HttpPost]
		public IActionResult Login(string username, string password)
		{
			var user = _loginService.ValidateUser(username, password);

			if (user != null)
			{
				// Session (simple style like your old system)
				HttpContext.Session.SetString("UserID", user.Id);
				HttpContext.Session.SetString("FullName", user.FullName);
				HttpContext.Session.SetString("DeptID", user.DeptId.ToString());
				HttpContext.Session.SetString("SectionID", user.SectionId.ToString());
				HttpContext.Session.SetString("trrsadmin", user.trrsadmin.ToString());
				HttpContext.Session.SetString("usersuniqueid", user.userid.ToString());


				// redirect to TRRF page (for now Index)
				return RedirectToAction("Index", "TRRF");
			}

			ViewBag.ErrorMessage = "Invalid username or password";
			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}


}
