using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace TechnicalRevisionRequestSystem.Filters
{
	public class SessionAuthorizeAttribute : ActionFilterAttribute
	{

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var userId = context.HttpContext.Session.GetString("UserID");

			if (string.IsNullOrEmpty(userId))
			{
				context.Result = new RedirectToActionResult("Login", "Auth", null);
				return;
			}

			base.OnActionExecuting(context);
		}
	}
}
