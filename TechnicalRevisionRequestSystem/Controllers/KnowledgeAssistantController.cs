using Microsoft.AspNetCore.Mvc;
using TechnicalRevisionRequestSystem.Models;
using TechnicalRevisionRequestSystem.Services.AI;

namespace TechnicalRevisionRequestSystem.Controllers
{
	public class KnowledgeAssistantController : Controller
	{
		private readonly IAIService _aiService;

		public KnowledgeAssistantController(IAIService aiService)
		{
			_aiService = aiService;
		}


		/// <summary>
		/// General AI (No TRRF Context)
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Ask(string question)
		{
			var answer = await _aiService.AskAsync(question);

			return Json(new
			{
				answer
			});
		}



		/// <summary>
		/// AI with Complete TRRF Context
		/// Specific TRRF ID
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> AskRequest([FromBody] AskRequestModel request)
		{
			var answer = await _aiService.AskAboutRequestAsync(
				request.TrrsId,
				request.Question);

			return Json(new
			{
				answer
			});
		}



		/// <summary>
		/// AI Knowledge Search
		/// Searches All Historical TRRF Records
		/// </summary>
		//[HttpPost]
		//public async Task<IActionResult> AskKnowledge([FromBody] AskKnowledgeModel request)
		//{
		//	var answer = await _aiService.AskKnowledgeAsync(
		//		request.Question);

		//	return Json(new
		//	{
		//		answer
		//	});
		//}

		[HttpPost]
		public async Task<IActionResult> AskKnowledge(
	[FromBody] AskKnowledgeModel request)
		{
			try
			{
				var answer = await _aiService.AskKnowledgeAsync(
					request.Question
				);

				return Json(new
				{
					answer = answer
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("========================================");
				Console.WriteLine("TRRS AI ERROR");
				Console.WriteLine("========================================");
				Console.WriteLine(ex.ToString());
				Console.WriteLine("========================================");

				return StatusCode(500, new
				{
					error = ex.Message
				});
			}
		}



		public IActionResult Index()
		{
			return View();
		}
	}
}