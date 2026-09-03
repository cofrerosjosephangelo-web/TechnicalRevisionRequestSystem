using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Data;
using System.Net.Mail;
using TechnicalRevisionRequestSystem.Filters;
using TechnicalRevisionRequestSystem.Models;

namespace TechnicalRevisionRequestSystem.Controllers.Administration
{
	

	public class AdminController : Controller
	{
		private readonly TRRSRepositoryInsert _TRRSInsert;
		private readonly TRRSRepositorySelect _TRRSSelect;
		private readonly TRRSRepositoryUpdate _TRRSUpdate;


		public AdminController(TRRSRepositoryInsert trrsRepositoryInsert, TRRSRepositorySelect trrsRepositorySelect, TRRSRepositoryUpdate tRRSUpdate)
		{
			_TRRSInsert = trrsRepositoryInsert;
			_TRRSSelect = trrsRepositorySelect;
			_TRRSUpdate = tRRSUpdate;
		}



		[SessionAuthorize]
		public IActionResult Customers()
		{
			var model = new ProductAdministrationViewModel
			{
				Products = _TRRSSelect.GetProductLookup(),
				Customers = _TRRSSelect.GetCustomerLookup()
			};

			return View(model);
		}

		[SessionAuthorize]
		public IActionResult Products()
		{
			var model = new ProductAdministrationViewModel
			{
				Products = _TRRSSelect.GetProductLookup(),
				Customers = _TRRSSelect.GetCustomerLookup()
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> InsertCustomer(string customerCode, string customerName)
		{
			var result = await _TRRSInsert.InsertCustomerAsync(customerCode, customerName);

			return Json(new
			{
				success = result.Success,
				message = result.Message,
				customerId = result.CustomerId
			});
		}

		[HttpPost]
		public async Task<IActionResult> UpdateCustomer(int customerId,string customerCode,string customerName)
		{
			var result = await _TRRSUpdate.UpdateCustomerAsync(
				customerId,
				customerCode,
				customerName);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}

		[HttpPost]
		public async Task<IActionResult> DeleteCustomer(int customerId)
		{
			var result = await _TRRSUpdate.DeleteCustomerAsync(customerId);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}








		[HttpPost]
		public async Task<IActionResult> InsertProduct(string productName,string? gml,string? modelDescription,int? customerId)
		{
			var result = await _TRRSInsert.InsertProductAsync(
				productName,
				gml,
				modelDescription,
				customerId);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}



		[HttpPost]
		public async Task<IActionResult> UpdateProduct(
	   int productId,
	   string productName,
	   string gml)
		{
			var result = await _TRRSUpdate.UpdateProductAsync(
				productId,
				productName,
				gml);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}



		[HttpPost]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			var result = await _TRRSUpdate.DeleteProductAsync(productId);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}

		[HttpPost]
		public async Task<IActionResult> RestoreProduct(int productId)
		{
			var result = await _TRRSUpdate.RestoreProductAsync(productId);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}

		[HttpPost]
		public async Task<IActionResult> UpdateProductBind(
int productId,
string productName,
string? gml,
string? modelDescription,
int? customerId)
		{
			var result = await _TRRSUpdate.UpdateProductBindAsync(
				productId,
				productName,
				gml,
				modelDescription,
				customerId);

			return Json(new
			{
				success = result.Success,
				message = result.Message
			});
		}




	}
}
