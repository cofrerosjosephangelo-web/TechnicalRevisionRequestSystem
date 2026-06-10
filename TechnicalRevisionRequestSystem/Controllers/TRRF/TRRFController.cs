using Microsoft.AspNetCore.Mvc;
using TechnicalRevisionRequestSystem.Filters;

namespace TechnicalRevisionRequestSystem.Controllers.TRRF
{
	public class TRRFController : Controller
	{

		[SessionAuthorize]
		public IActionResult Index()
		{
			return View();
		}
	}
}
