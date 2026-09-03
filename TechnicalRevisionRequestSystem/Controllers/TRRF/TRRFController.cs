using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Data;
using System.Net.Mail;	
using System.Threading.Tasks;
using TechnicalRevisionRequestSystem.Filters;
using TechnicalRevisionRequestSystem.Models;

namespace TechnicalRevisionRequestSystem.Controllers.TRRF
{

	public class TRRFController : Controller
	{

		private readonly TRRSRepositoryInsert _TRRSInsert;
		private readonly TRRSRepositorySelect _TRRSSelect;
		private readonly TRRSRepositoryUpdate _TRRSUpdate;

		public TRRFController(TRRSRepositoryInsert trrsRepositoryInsert, TRRSRepositorySelect trrsRepositorySelect, TRRSRepositoryUpdate tRRSUpdate)
		{
			_TRRSInsert = trrsRepositoryInsert;
			_TRRSSelect = trrsRepositorySelect;
			_TRRSUpdate = tRRSUpdate;
		}

		[SessionAuthorize]
		public IActionResult Index()
		{
			return View();
		}





		#region "INSERT"

		[HttpPost]
		public async Task<IActionResult> InsertRequest([FromForm] InsertTrrsRequestModel model)
		{

			Console.WriteLine("Controller entered");

			Console.WriteLine(model == null);

			Console.WriteLine(model.RootCauseAttachments?.Length);

			Console.WriteLine(model.ActionPlanAttachments?.Length);

			model.CreateUser = HttpContext.Session.GetString("FullName") ?? "";
			model.CreateUserName = HttpContext.Session.GetString("FullName") ?? "";

			model.CreateUserId = HttpContext.Session.GetString("UserID") ?? "";

			model.DeptId = int.TryParse(HttpContext.Session.GetString("DeptID"), out var deptId)
				? deptId
				: 0;

			model.SectionId = int.TryParse(HttpContext.Session.GetString("SectionID"), out var sectionId)
				? sectionId
				: 0;

			var trrsId = await _TRRSInsert.InsertRequestAsync(model);

			if (model.RootCauseAttachments != null)
			{
				foreach (var file in model.RootCauseAttachments)
				{
					using var ms = new MemoryStream();

					await file.CopyToAsync(ms);

					await _TRRSInsert.InsertRootCauseAttachmentAsync(
						trrsId,
						file.FileName,
						ms.ToArray());
				}
			}

			if (model.ActionPlanAttachments != null)
			{
				foreach (var file in model.ActionPlanAttachments)
				{
					using var ms = new MemoryStream();

					await file.CopyToAsync(ms);

					await _TRRSInsert.InsertActionPlanAttachmentAsync(
						trrsId,
						file.FileName,
						ms.ToArray());
				}
			}


			if(model.MeetingMinutesAttachments != null)
			{
				foreach (var file in model.MeetingMinutesAttachments)
				{
					using var ms = new MemoryStream();

					await file.CopyToAsync(ms);

					await _TRRSInsert.InsertMomAttachmentAsync(
					trrsId,
					file.FileName,
					ms.ToArray(),
					model.MeetingMinutesRemarksTxt
				);
				}
			}

			return Json(new
			{
				success = true,
				trrsId,
				message = "New TRRF submitted successfully!"
			});
		}


		[HttpPost]
		public async Task<IActionResult> UpdateApprovalNotes(int trrsId,int approvalId,string notes)
		{
			var result = await _TRRSInsert.UpdateApprovalNotesAsync(
				trrsId,
				approvalId,
				notes
			);

			if (result)
			{
				return Json(new
				{
					success = true,
					message = "Notes updated successfully."
				});
			}

			return Json(new
			{
				success = false,
				message = "Unable to update notes."
			});
		}










		#endregion


		#region "SELECTABLES"

		//ALL SELECTABLES HERE



		[HttpGet]
		public JsonResult GetCustomerLookup()
		{
			var list = _TRRSSelect.GetCustomerLookup();
			return Json(list);
		}

		[HttpGet]
		public JsonResult GetProductLookup()
		{
			var list = _TRRSSelect.GetProductLookup();
			return Json(list);
		}

		[HttpGet]
		public JsonResult GetTrrsControl()
		{
			var list = _TRRSSelect.GetTrrsControl();
			return Json(list);
		}

		[HttpGet]
		public async Task<IActionResult> GetRequestList()
		{
			var data = await _TRRSSelect.GetRequestListAsync();

			return Json(new
			{
				data = data,
				last_page = 1
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetMyApprovalRequestList()
		{
			int deptId = Convert.ToInt32(HttpContext.Session.GetString("DeptID"));
			int sectionId = Convert.ToInt32(HttpContext.Session.GetString("SectionID"));

			var data = await _TRRSSelect.GetMyApprovalRequestListAsync(deptId, sectionId);

			return Json(new
			{
				data = data,
				last_page = 1
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetRequestApprovals(int trrsId)
		{
			var data = await _TRRSSelect.GetRequestApprovalsAsync(trrsId);

			return Json(new
			{
				data = data,
				last_page = 1
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetRequestDetails(int trrsId)
		{
			var result = await _TRRSSelect.GetRequestDetailsAsync(trrsId);

			if (result == null)
			{
				return NotFound();
			}

			return Json(result);
		}

		[HttpGet]
		public IActionResult GetApprovalHistory(int trrsId,int approverPart)
		{
			var result = _TRRSSelect.GetApprovalHistory(trrsId,approverPart);

			return Json(result);
		}


		[HttpGet]
		public async Task<IActionResult> GetRevisionHistory(int trrsId, int? deviationId)
		{
			try
			{
				var result = await _TRRSSelect.GetDeviationDetails(trrsId, deviationId);

				return Json(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.ToString());
			}
		}

		[HttpGet]
		public IActionResult GetApproverAttachments(int trrsId,int approvingPart)
		{
			try
			{
				var attachments = _TRRSSelect.GetApproverAttachments(
					trrsId,
					approvingPart);

				return Json(new
				{
					success = true,
					data = attachments
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		public IActionResult GetProductStatus(int trrsId)
		{
			try
			{
				var result = _TRRSSelect.GetProductStatus(trrsId);

				if (result == null)
				{
					return Json(new
					{
						success = false,
						message = "Product Status not found."
					});
				}

				return Json(new
				{
					success = true,
					data = result
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetActualInfo(int trrsId)
		{
			try
			{
				var result = await _TRRSSelect.GetActualActivityInfo(trrsId);	
				if (result == null)
				{
					return Json(new
					{
						success = false,
						message = "Actual Activity Info not found."
					});
				}

				return Json(new
				{
					success = true,
					data = result
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}


		[HttpGet]
		public IActionResult GetAttachments(int trrsId, string type)
		{
			try
			{
				object attachments;

				switch (type?.ToLower())
				{
					case "rootcause":
						attachments = _TRRSSelect.GetRootCauseAttachments(trrsId);
						break;

					case "actionplan":
						attachments = _TRRSSelect.GetActionPlanAttachments(trrsId);
						break;

					case "mom":
						attachments = _TRRSSelect.GetMomAttachments(trrsId);
						break;

					default:
						return Json(new
						{
							success = false,
							message = "Invalid attachment type."
						});
				}

				return Json(new
				{
					success = true,
					data = attachments
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}


		[HttpGet]
		public IActionResult PreviewAttachment(int attachmentId, string type)
		{
			byte[] fileData = null;
			string fileName = "";

			switch (type?.ToLower())
			{
				case "rootcause":

					var rc = _TRRSSelect.GetRootCauseAttachment(attachmentId);

					if (rc == null)
						return NotFound();

					fileData = rc.file_data;
					fileName = rc.file_name;

					break;

				case "actionplan":

					var ap = _TRRSSelect.GetActionPlanAttachment(attachmentId);

					if (ap == null)
						return NotFound();

					fileData = ap.file_data;
					fileName = ap.file_name;

					break;

				case "mom":

					var mom = _TRRSSelect.GetMomAttachment(attachmentId);

					if (mom == null)
						return NotFound();

					fileData = mom.file_data;
					fileName = mom.file_name;

					break;

				case "approver":

					var app = _TRRSSelect.GetApproverAttachmentById(attachmentId);

					fileData = app.file_data;
					fileName = app.file_name;

					break;


				default:
					return BadRequest("Invalid attachment type.");
			}

			return File(
				fileData,
				GetContentType(fileName),
				enableRangeProcessing: true
			);
		}


		[HttpGet]
		public IActionResult DownloadAttachment(int attachmentId, string type)
		{
			byte[] fileData = null;
			string fileName = "";

			switch (type?.ToLower())
			{
				case "rootcause":

					var rc = _TRRSSelect.GetRootCauseAttachment(attachmentId);

					if (rc == null)
						return NotFound();

					fileData = rc.file_data;
					fileName = rc.file_name;

					break;

				case "actionplan":

					var ap = _TRRSSelect.GetActionPlanAttachment(attachmentId);

					if (ap == null)
						return NotFound();

					fileData = ap.file_data;
					fileName = ap.file_name;

					break;

				case "mom":

					var mom = _TRRSSelect.GetMomAttachment(attachmentId);

					if (mom == null)
						return NotFound();

					fileData = mom.file_data;
					fileName = mom.file_name;

					break;

				case "approver":
					var app = _TRRSSelect.GetApproverAttachmentById(attachmentId);

					fileData = app.file_data;
					fileName = app.file_name;

					break;

				default:
					return BadRequest("Invalid attachment type.");
			}

			return File(
				fileData,
				"application/octet-stream",
				fileName
			);
		}

		private string GetContentType(string fileName)
		{
			var provider = new FileExtensionContentTypeProvider();

			if (provider.TryGetContentType(fileName, out string contentType))
			{
				return contentType;
			}

			return "application/octet-stream";
		}


		[HttpGet]
		public async Task<IActionResult> GetProductionSchedule(int trrsId)
		{
			try
			{
				var result = await _TRRSSelect.GetProductionSchedule(trrsId);

				if (result == null)
				{
					return Json(new
					{
						success = false,
						message = "Production Schedule not found."
					});
				}

				return Json(new
				{
					success = true,
					data = result
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetProductResult(int trrsId)
		{
			try
			{
				var result = await _TRRSSelect.GetEvaluationResultAsync(trrsId);	
				if (result == null)
				{
					return Json(new
					{
						success = false,
						message = "Product Result not found."
					});
				}

				return Json(new
				{
					success = true,
					data = result
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}


		[HttpGet]
		public async Task<IActionResult> GetQualityVerification(int trrsId)
		{
			try
			{
				var result = await _TRRSSelect.GetQualityVerification(trrsId);
				if (result == null)
				{
					return Json(new
					{
						success = false,
						message = "Product Result not found."
					});
				}

				return Json(new
				{
					success = true,
					data = result
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}


		[HttpGet]
		public async Task<IActionResult> GetDashboardSummary(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetDashboardSummaryAsync(year, month);

			if (dt.Rows.Count == 0)
			{
				return Json(new
				{
					success = false,
					message = "No dashboard data found."
				});
			}

			DataRow row = dt.Rows[0];

			return Json(new
			{
				success = true,

				totalRequests = Convert.ToInt32(row["TotalRequests"]),
				pdRequests = Convert.ToInt32(row["PDRequests"]),
				mpRequests = Convert.ToInt32(row["MPRequests"]),
				totalCustomers = Convert.ToInt32(row["TotalCustomers"]),
				totalProducts = Convert.ToInt32(row["TotalProducts"]),
				totalMolds = Convert.ToInt32(row["TotalMolds"]),
				averageRevision = Convert.ToDecimal(row["AverageRevision"])
			});
		}


		[HttpGet]
		public async Task<IActionResult> GetMonthlyRequests(int year)
		{
			var dt = await _TRRSSelect.GetMonthlyRequestsAsync(year);

			var data = dt.AsEnumerable().Select(r => new
			{
				monthNo = Convert.ToInt32(r["MonthNo"]),
				monthName = r["MonthName"].ToString(),
				totalRequests = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}


		[HttpGet]
		public async Task<IActionResult> GetRequestsByType(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetRequestsByTypeAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				name = r["trrs_type"].ToString(),
				value = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetRequestsByCustomer(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetRequestsByCustomerAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				customerName = r["customer_name"].ToString(),
				totalRequests = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetRequestsByProduct(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetRequestsByProductAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				productName = r["product_name"].ToString(),
				totalRequests = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}


		[HttpGet]
		public async Task<IActionResult> GetRevisionDistribution(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetRevisionDistributionAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				revision = Convert.ToInt32(r["Revision"]),
				totalRequests = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetRequestsByMold(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetRequestsByMoldAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				moldNo = r["mold_no"].ToString(),
				totalRequests = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}

		[HttpGet]
		public async Task<IActionResult> GetDeviationAnalytics(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetParentChildSummaryAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				name = r["RequestType"].ToString(),
				value = Convert.ToInt32(r["TotalRequests"])
			});

			return Json(data);
		}


		[HttpGet]
		public async Task<IActionResult> GetTopRequestedProducts(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetTopRequestedProductsAsync(year, month);

			var data = dt.AsEnumerable().Select(r => new
			{
				no = Convert.ToInt32(r["No."]),
				customer = r["Customer"].ToString(),
				productName = r["Product Name"].ToString(),
				moldNo = r["Mold No."].ToString(),
				trrfCount = Convert.ToInt32(r["TRRF Count"])
			});

			return Json(new
			{
				data = data
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetDetailedBreakdown(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetDetailedBreakdownAsync(year, month);

			var data = dt.AsEnumerable().Select(r => dt.Columns.Cast<DataColumn>()
				.ToDictionary(
					c => c.ColumnName,
					c => r[c]
				));

			return Json(new
			{
				data = data
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetMonthlyTrend(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetMonthlyTrendAsync(year, month);

			var data = dt.AsEnumerable().Select(r => dt.Columns.Cast<DataColumn>()
				.ToDictionary(
					c => c.ColumnName,
					c => r[c]
				));

			return Json(new
			{
				data = data
			});
		}


		[HttpGet]
		public async Task<IActionResult> GetTRRFDistributionByLifeUtilization(int? year, int? month)
		{
			var dt = await _TRRSSelect.GetTRRFDistributionByLifeUtilizationAsync(year, month);

			var data = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>()
				.ToDictionary(
					column => column.ColumnName,
					column => row[column]
				));

			return Json(new { data = data });
		}

		[HttpGet]
		public async Task<IActionResult> ExportExcel()
		{
			// TODO: Replace this with your actual method
			List<TrrsExportModel> data = await _TRRSSelect.GetRequestListReportAsync();
			DataTable trackingTable = await _TRRSSelect.GetApprovalsHorizontalAsync(null);

			using var workbook = new XLWorkbook();
			var ws = workbook.Worksheets.Add("TRRS Report");
			var trackingWs = workbook.Worksheets.Add("Tracking");

			// Report Title
			ws.Cell(1, 1).Value = "TECHNICAL REVISION REQUEST SYSTEM";
			ws.Range(1, 1, 1, 13).Merge();
			ws.Range(1, 1, 1, 13).Style.Font.Bold = true;
			ws.Range(1, 1, 1, 13).Style.Font.FontSize = 18;
			ws.Range(1, 1, 1, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

			ws.Cell(2, 1).Value = "TRRS REQUEST REPORT";
			ws.Range(2, 1, 2, 13).Merge();
			ws.Range(2, 1, 2, 13).Style.Font.Bold = true;
			ws.Range(2, 1, 2, 13).Style.Font.FontSize = 14;
			ws.Range(2, 1, 2, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

			ws.Cell(4, 1).Value = "Generated By:";
			ws.Cell(4, 2).Value = HttpContext.Session.GetString("FullName");

			ws.Cell(5, 1).Value = "Generated On:";
			ws.Cell(5, 2).Value = DateTime.Now;


			trackingWs.Cell(1, 1).Value = "TECHNICAL REVISION REQUEST SYSTEM";
			trackingWs.Range(1, 1, 1, trackingTable.Columns.Count).Merge();

			trackingWs.Range(1, 1, 1, trackingTable.Columns.Count).Style.Font.Bold = true;
			trackingWs.Range(1, 1, 1, trackingTable.Columns.Count).Style.Font.FontSize = 18;
			trackingWs.Range(1, 1, 1, trackingTable.Columns.Count).Style.Alignment.Horizontal =
				XLAlignmentHorizontalValues.Center;

			trackingWs.Cell(2, 1).Value = "TRRS TRACKING REPORT";
			trackingWs.Range(2, 1, 2, trackingTable.Columns.Count).Merge();

			trackingWs.Range(2, 1, 2, trackingTable.Columns.Count).Style.Font.Bold = true;
			trackingWs.Range(2, 1, 2, trackingTable.Columns.Count).Style.Font.FontSize = 14;
			trackingWs.Range(2, 1, 2, trackingTable.Columns.Count).Style.Alignment.Horizontal =
				XLAlignmentHorizontalValues.Center;

			trackingWs.Cell(4, 1).Value = "Generated By:";
			trackingWs.Cell(4, 2).Value = HttpContext.Session.GetString("FullName");

			trackingWs.Cell(5, 1).Value = "Generated On:";
			trackingWs.Cell(5, 2).Value = DateTime.Now;

			int trackingRow = 7;

			for (int c = 0; c < trackingTable.Columns.Count; c++)
			{
				var cell = trackingWs.Cell(trackingRow, c + 1);

				cell.Value = trackingTable.Columns[c].ColumnName;

				cell.Style.Font.Bold = true;
				cell.Style.Font.FontColor = XLColor.White;
				cell.Style.Fill.BackgroundColor = XLColor.DarkBlue;

				cell.Style.Alignment.Horizontal =
					XLAlignmentHorizontalValues.Center;
			}

			trackingRow++;

			foreach (DataRow dr in trackingTable.Rows)
			{
				for (int c = 0; c < trackingTable.Columns.Count; c++)
				{
					trackingWs.Cell(trackingRow, c + 1).Value =
						dr[c]?.ToString();
				}

				trackingRow++;
			}

			trackingWs.RangeUsed().Style.Border.OutsideBorder =
	XLBorderStyleValues.Thin;

			trackingWs.RangeUsed().Style.Border.InsideBorder =
				XLBorderStyleValues.Thin;

			trackingWs.Columns().AdjustToContents();

			trackingWs.SheetView.FreezeRows(7);

			trackingWs.Range(
	7,                              // Header row
	1,
	trackingRow - 1,                // Last data row
	trackingTable.Columns.Count     // Last column
).SetAutoFilter();

			trackingWs.PageSetup.PageOrientation =
				XLPageOrientation.Landscape;

			trackingWs.PageSetup.FitToPages(1, 0);

			// Header row
			int row = 7;

			string[] headers =
			{
		"Control No",
		"TRRF No",
		"Version",
		"Type",
		"Customer",
		"Product",
		"Mold No",
		"Part Model",
		"Problem",
		"Machine",
		"Submitted By",
		"Status",
		"Date Prepared"
	};

			for (int i = 0; i < headers.Length; i++)
			{
				var cell = ws.Cell(row, i + 1);
				cell.Value = headers[i];
				cell.Style.Font.Bold = true;
				cell.Style.Fill.BackgroundColor = XLColor.DarkBlue;
				cell.Style.Font.FontColor = XLColor.White;
				cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}

			row++;

			foreach (var item in data)
			{
				ws.Cell(row, 1).Value = item.ControlNo;
				ws.Cell(row, 2).Value = item.TRRFNo;
				ws.Cell(row, 3).Value = item.RevisionNo;
				ws.Cell(row, 4).Value = item.Type;
				ws.Cell(row, 5).Value = item.Customer;
				ws.Cell(row, 6).Value = item.Product;
				ws.Cell(row, 7).Value = item.MoldNo;
				ws.Cell(row, 8).Value = item.PartModelNo;
				ws.Cell(row, 9).Value = item.Problem;
				ws.Cell(row, 10).Value = item.MachineName;
				ws.Cell(row, 11).Value = item.SubmittedBy;
				ws.Cell(row, 12).Value = item.CurrentStatus;
				ws.Cell(row, 13).Value = item.DatePrepared;
				ws.Cell(row, 13).Style.DateFormat.Format = "MM-dd-yyyy";

				row++;
			}

			// Borders
			ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
			ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

			// Auto width
			ws.Columns().AdjustToContents();

			// Freeze header
			ws.SheetView.FreezeRows(7);

			// Auto filter
			ws.Range(7, 1, row - 1, headers.Length).SetAutoFilter();

			// Landscape
			ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
			ws.PageSetup.FitToPages(1, 0);

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);

			return File(
				stream.ToArray(),
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				$"TRRS_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
		}



		[HttpGet]
		public async Task<IActionResult> GetProductsByCustomer(int customerId)
		{
			var products = await _TRRSSelect.GetProductsByCustomerAsync(customerId);

			return Json(products);
		}



		[HttpGet]
		public async Task<IActionResult> GetTRRFDetailsByStageId(int stageId, int? year, int? month)
		{
			var dt = await _TRRSSelect.GetTRRFDetailsByStageIdAsync(stageId, year, month);

			var data = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>()
				.ToDictionary(
					column => column.ColumnName,
					column => row[column]
				));

			return Json(new { data = data });
		}

		#endregion


		#region "UPDATES"

		[HttpPost]
		public async Task<IActionResult> ApproveRequest(
	ApproveRequestModel model,
	List<IFormFile> files)
		{
			try
			{
				model.approved_by = HttpContext.Session.GetString("FullName");

				var result = _TRRSUpdate.ApproveRequest(model);

				if (!result.Success)
				{
					return Json(new
					{
						success = false,
						message = "Failed to approve request."
					});
				}

				int newtrrsid = result.NewTRRSId ?? 0;

				// ============================================
				// PD Workflow
				// ============================================

				if (model.trrs_type == "PD")
				{
					switch (model.approving_part)
					{
						// Product Status
						case 9:

							await _TRRSInsert.SaveProductStatus(new ProductStatusModel
							{
								trrs_id = model.trrs_id,
								is_rush = model.is_rush,
								is_next_production = model.is_next_production,
								status_as_of = model.status_as_of,
								required_date = model.required_date,
								status_details = model.status_details
							});

							break;

						// Design Change
						case 10:

							await _TRRSInsert.InsertActualActivityInfo(
								new ActualActivityInfoModel
								{
									trrs_id = model.trrs_id,
									datereceived = model.datereceived,
									dateaccomplishedchange = model.dateaccomplishedchange,
									designchangedetails = model.designchangedetails
								});

							break;

						// Part Modified
						case 11:

							_TRRSUpdate.UpdatePartModifiedInfo(
								new ActualActivityInfoModel
								{
									trrs_id = model.trrs_id,
									dateaccomplishedmodified = model.dateaccomplishedmodified,
									partmodifieddetails = model.partmodifieddetails
								});

							break;

						// Mold Result
						case 12:

							_TRRSUpdate.UpdateMoldResultInfo(
								new ActualActivityInfoModel
								{
									trrs_id = model.trrs_id,
									moldresult = model.moldresult,
									dateaccomplishedresult = model.dateaccomplishedresult
								});

							break;

						// Production Schedule
						case 14:

							await _TRRSInsert.InsertProductionSchedule(
								new ProductionScheduleModel
								{
									trrs_id = model.trrs_id,
									proddatereceived = model.proddatereceived,
									prodtesting = model.prodtesting
								});

							break;

						// Result Evaluation
						case 15:

							await _TRRSInsert.SaveEvaluationResult(
								new EvaluationResultModel
								{
									trrs_id = model.trrs_id,
									productresult = model.productresult,
									isapproved = model.isapproved
								});

							break;

						// Quality Verification
						case 16:

							await _TRRSInsert.SaveQualityVerification(
								new QualityVerificationModel
								{
									trrs_id = model.trrs_id,
									job_order = model.job_order
								});

							break;
					}
				}

				// ============================================
				// MP Workflow
				// ============================================

				else if (model.trrs_type == "MP")
				{
					switch (model.approving_part)
					{
						// Product Status
						case 8:

							await _TRRSInsert.SaveProductStatus(new ProductStatusModel
							{
								trrs_id = model.trrs_id,
								is_rush = model.is_rush,
								is_next_production = model.is_next_production,
								status_as_of = model.status_as_of,
								required_date = model.required_date,
								status_details = model.status_details
							});

							break;

						// Design Change
						case 9:

							await _TRRSInsert.InsertActualActivityInfo(
								new ActualActivityInfoModel
								{
									trrs_id = model.trrs_id,
									datereceived = model.datereceived,
									dateaccomplishedchange = model.dateaccomplishedchange,
									designchangedetails = model.designchangedetails
								});

							break;

						// Part Modified
						case 10:

							_TRRSUpdate.UpdatePartModifiedInfo(
								new ActualActivityInfoModel
								{
									trrs_id = model.trrs_id,
									dateaccomplishedmodified = model.dateaccomplishedmodified,
									partmodifieddetails = model.partmodifieddetails
								});

							break;

						// Mold Result
						case 11:

							_TRRSUpdate.UpdateMoldResultInfo(
								new ActualActivityInfoModel
								{
									trrs_id = model.trrs_id,
									moldresult = model.moldresult,
									dateaccomplishedresult = model.dateaccomplishedresult
								});

							break;

						// Production Schedule
						case 13:

							await _TRRSInsert.InsertProductionSchedule(
								new ProductionScheduleModel
								{
									trrs_id = model.trrs_id,
									proddatereceived = model.proddatereceived,
									prodtesting = model.prodtesting
								});

							break;

						// Result Evaluation + Quality Verification
						case 14:

							await _TRRSInsert.SaveEvaluationResult(
								new EvaluationResultModel
								{
									trrs_id = model.trrs_id,
									productresult = model.productresult,
									isapproved = model.isapproved
								});

							await _TRRSInsert.SaveQualityVerification(
								new QualityVerificationModel
								{
									trrs_id = model.trrs_id,
									job_order = model.job_order
								});

							break;


						//deviation

						case 15:

							if (model.deviation_id.HasValue)
							{
								await _TRRSInsert.InsertDeviationDetail(
									new DeviationDetailModel
									{
										deviation_id = model.deviation_id,
										deviation_details = model.deviation_details,
										deviation_date = model.deviation_date
									});
							}

							break;
					}
				}

				// ============================================
				// Attachments
				// ============================================

				if (files != null && files.Any())
				{
					foreach (var file in files)
					{
						if (file.Length <= 0)
							continue;

						using var ms = new MemoryStream();

						await file.CopyToAsync(ms);

						await _TRRSInsert.InsertApprovalAttachmentAsync(
							model.trrs_id,
							model.approving_part,
							file.FileName,
							ms.ToArray());
					}
				}

				if (model.RootCauseAttachments != null)
				{
					foreach (var file in model.RootCauseAttachments)
					{
						using var ms = new MemoryStream();

						await file.CopyToAsync(ms);

						await _TRRSInsert.InsertRootCauseAttachmentAsync(
							newtrrsid,
							file.FileName,
							ms.ToArray());
					}
				}

				if (model.ActionPlanAttachments != null)
				{
					foreach (var file in model.ActionPlanAttachments)
					{
						using var ms = new MemoryStream();

						await file.CopyToAsync(ms);

						await _TRRSInsert.InsertActionPlanAttachmentAsync(
							newtrrsid,
							file.FileName,
							ms.ToArray());
					}
				}

				return Json(new
				{
					success = true,
					message = "Request approved successfully."
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = ex.Message
				});
			}
		}


		


		#endregion


	}
}
