using System.Data;
using Microsoft.Data.SqlClient;

using TechnicalRevisionRequestSystem.Models;

namespace TechnicalRevisionRequestSystem.Models
{
	public class SSQLDB : TRRSRepositorySelect
	{

		private readonly string _connectionString;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SSQLDB(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = configuration.GetConnectionString("TRRSConnection");
			_httpContextAccessor = httpContextAccessor;
		}


		public async Task<TrrsRequestDetailsModel?> GetRequestDetailsAsync(int trrsId)
		{
			using var con = new SqlConnection(_connectionString);
			using var cmd = new SqlCommand("trrs_sp_get_request_details", con);

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@trrs_id", trrsId);

			await con.OpenAsync();

			using var reader = await cmd.ExecuteReaderAsync();

			if (await reader.ReadAsync())
			{
				return new TrrsRequestDetailsModel
				{
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					TrrfNo = reader["trrf_no"]?.ToString(),
					DatePrepared = reader["date_prepared"] as DateTime?,
					TrrsType = reader["trrs_type"]?.ToString(),

					CustomerId = Convert.ToInt32(reader["customer_id"]),
					CustomerName = reader["customer_name"]?.ToString(),

					ProductId = Convert.ToInt32(reader["product_id"]),
					ProductName = reader["product_name"]?.ToString(),

					MoldNo = reader["mold_no"]?.ToString(),
					PartModelNo = reader["part_model_no"]?.ToString(),

					EncounteredProblem = reader["encountered_problem"]?.ToString(),
					MachineName = reader["machine_name"]?.ToString(),

					MoldToolLife = reader["mold_tool_life"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["mold_tool_life"]),

					RcWearTear = Convert.ToBoolean(reader["rc_wear_tear"]),
					RcMachineError = Convert.ToBoolean(reader["rc_machine_error"]),
					RcDesignError = Convert.ToBoolean(reader["rc_design_error"]),
					RcFabricationError = Convert.ToBoolean(reader["rc_fabrication_error"]),
					RcEffectOfPrevImprovement = Convert.ToBoolean(reader["rc_effect_of_prev_improvement"]),
					RcCustomerRequirement = Convert.ToBoolean(reader["rc_customer_requirement"]),

					RcDetails = reader["rc_details"]?.ToString(),

					ApRepair = Convert.ToBoolean(reader["ap_repair"]),
					ApAdjustment = Convert.ToBoolean(reader["ap_adjustment"]),
					ApRevision = Convert.ToBoolean(reader["ap_revision"]),
					ApReplacement = Convert.ToBoolean(reader["ap_replacement"]),
					ApTrialTesting = Convert.ToBoolean(reader["ap_trial_testing"]),

					ApDetails = reader["ap_details"]?.ToString(),

					HasSpare = Convert.ToBoolean(reader["has_spare"]),

					Quantity = reader["quantity"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["quantity"]),

					ControlDisplay = reader["control_display"]?.ToString() ,
					rcattachment = reader["rcattachment"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["rcattachment"]),
					apattachment = reader["apattachment"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["apattachment"]),
					momattachment = reader["momattachment"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["momattachment"]),
					momremarks = reader["momremarks"]?.ToString()

				};
			}

			return null;
		}


		public List<CustomerLookupModel> GetCustomerLookup()
		{
			var list = new List<CustomerLookupModel>();

			using (var conn = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand(@"
        SELECT *
        FROM [trrs_db_new].[dbo].[vw_customer_lookup]
        ORDER BY customer_display_name", conn))
			{
				conn.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new CustomerLookupModel
						{
							customerId = reader["customer_id"] != DBNull.Value
												? Convert.ToInt32(reader["customer_id"])
												: 0,
							customerCode = reader["customer_code"]?.ToString(),
							customersName = reader["customer_name"]?.ToString(),
							customerName = reader["customer_display_name"]?.ToString()
						});
					}
				}
			}

			return list;
		}

		public List<ProductLookupModel> GetProductLookup()
		{
			var list = new List<ProductLookupModel>();

			using (var conn = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand(@"
		SELECT *
		FROM [trrs_db_new].[dbo].[vw_product_lookup]
		ORDER BY product_name", conn))
			{
				conn.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new ProductLookupModel
						{
							productId = reader["prod_id"] != DBNull.Value
								? Convert.ToInt32(reader["prod_id"])
								: 0,

							productName = reader["product_name"]?.ToString(),
							gml = reader["gml"] != DBNull.Value
								? Convert.ToInt32(reader["gml"])
								: 0,
							customerid = reader["customer_id"] != DBNull.Value
								? Convert.ToInt32(reader["customer_id"])
								: (int?)null,
							modelpartno = reader["model_part_no"]?.ToString() ,
							isdeleted = reader["isdeleted"] != DBNull.Value
								? Convert.ToInt32(reader["isdeleted"])
								: (int?)null
						});
					}
				}
			}

			return list;
		}

		public List<TrrsControlModel> GetTrrsControl()
		{
			var list = new List<TrrsControlModel>();

			using (var conn = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand(@"
		SELECT *
		FROM [trrs_db_new].[dbo].[vw_trrs_control]
		ORDER BY id", conn))
			{
				conn.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new TrrsControlModel
						{
							id = reader["id"] != DBNull.Value
								? Convert.ToInt32(reader["id"])
								: 0,

							controlName = reader["control_name"]?.ToString(),
							controlCode = reader["control_code"]?.ToString(),

							controlNumber = reader["control_number"] != DBNull.Value
								? Convert.ToInt32(reader["control_number"])
								: 0,

							controlDisplay = reader["control_display"]?.ToString()
						});
					}
				}
			}

			return list;
		}


		public async Task<List<TrrsRequestListModel>> GetRequestListAsync()
		{
			var requests = new List<TrrsRequestListModel>();

			using var con = new SqlConnection(_connectionString);
			using var cmd = new SqlCommand("trrs_sp_get_request_list", con);

			cmd.CommandType = CommandType.StoredProcedure;

			await con.OpenAsync();

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				requests.Add(new TrrsRequestListModel
				{
					controlno = reader["controlno"]?.ToString(),
					id = reader["id"] != DBNull.Value
								? Convert.ToInt32(reader["id"])
								: 0,
					trrfno = reader["trrfno"]?.ToString(),
					customername = reader["customername"]?.ToString(),
					trrstype = reader["trrstype"]?.ToString(),
					productname = reader["productname"]?.ToString(),
					moldno	 = reader["moldno"] != DBNull.Value
								? Convert.ToInt32(reader["moldno"])
								: 0,
					partmodelno = reader["partmodelno"]?.ToString(),
					machinename = reader["machinename"]?.ToString(),
					problem = reader["problem"]?.ToString(),
					submittedby = reader["submittedby"]?.ToString(),
					dateprepared = reader["dateprepared"] != DBNull.Value
								? Convert.ToDateTime(reader["dateprepared"])
								: DateTime.MinValue,
					currentstatus = reader["task"]?.ToString(),
					currentpart = reader["currentpart"] != DBNull.Value
								? Convert.ToInt32(reader["currentpart"])
								: 0	,
					revisionno = reader["trrsversion"] != DBNull.Value
								? Convert.ToInt32(reader["trrsversion"])
								: 0	,
					deviationid = reader["deviationid"] != DBNull.Value
								? Convert.ToInt32(reader["deviationid"])
								: (int?)null,
					moldreference = reader["moldreference"]?.ToString()

				});
			}

			return requests;
		}

		public async Task<List<TrrsRequestListModel>> GetMyApprovalRequestListAsync(int deptId, int sectionId)
		{
			var list = new List<TrrsRequestListModel>();

			using (SqlConnection con = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_get_my_approval_request_list", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@dept_id", deptId);
					cmd.Parameters.AddWithValue("@section_id", sectionId);

					await con.OpenAsync();

					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							list.Add(new TrrsRequestListModel
							{
								controlno = reader["controlno"]?.ToString(),
								trrfno = reader["trrfno"]?.ToString(),
								customername = reader["customername"]?.ToString(),
								productname = reader["productname"]?.ToString(),
								moldno = reader["moldno"] != DBNull.Value
								? Convert.ToInt32(reader["moldno"])
								: 0,
								partmodelno = reader["partmodelno"]?.ToString(),
								machinename = reader["machinename"]?.ToString(),
								problem = reader["problem"]?.ToString(),
								submittedby = reader["submittedby"]?.ToString(),
								dateprepared = reader["dateprepared"] != DBNull.Value
								? Convert.ToDateTime(reader["dateprepared"])
								: DateTime.MinValue,
								id = Convert.ToInt32(reader["id"]),
								trrstype = reader["trrstype"]?.ToString(),
								currentstatus = reader["task"]?.ToString(),
								currentpart = Convert.ToInt32(reader["currentpart"]),
								revisionno = Convert.ToInt32(reader["trrsversion"]),
								deviationid = reader["deviationid"] == DBNull.Value
									? 0
									: Convert.ToInt32(reader["deviationid"])
							});
						}
					}
				}
			}

			return list;
		}


		public async Task<List<ApprovalModel>> GetRequestApprovalsAsync(int trrsId)
		{
			var result = new List<ApprovalModel>();

			using var con = new SqlConnection(_connectionString);
			using var cmd = new SqlCommand("trrs_sp_select_request_approvals", con);

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@trrs_id", trrsId);

			await con.OpenAsync();

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				result.Add(new ApprovalModel
				{
					part = Convert.ToInt32(reader["part"]),
					role = reader["role"]?.ToString(),
					approvedby = reader["approvedby"]?.ToString(),
					approveddate = reader["approveddate"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["approveddate"]),
					remarks = reader["remarks"]?.ToString(),
					done = reader["done"] != DBNull.Value
						? Convert.ToInt32(reader["done"])
						: 0,
					forapprove = reader["forapprove"] != DBNull.Value
						? Convert.ToInt32(reader["forapprove"])
						: 0,
					deptid = Convert.ToInt32(reader["deptid"]),
					sectionid = Convert.ToInt32(reader["sectionid"]),
					id = Convert.ToInt32(reader["id"]),
					trrstype = reader["trrstype"]?.ToString(),
					deviationid = reader["deviationid"] == DBNull.Value
						? null
						: reader["deviationid"].ToString()	,
					notes = reader["notes"]?.ToString(),
					approvalId = Convert.ToInt32(reader["approval_Id"])
				});
			}

			return result;
		}


		public List<ApprovalHistoryModel> GetApprovalHistory(int trrsId,int approverPart)
		{
			List<ApprovalHistoryModel> list = new();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(
					"trrs_sp_get_approval_history", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrsid", trrsId);
					cmd.Parameters.AddWithValue("@approverPart", approverPart);

					conn.Open();

					using (SqlDataReader dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							list.Add(new ApprovalHistoryModel
							{
								approval_id = Convert.ToInt32(dr["approval_id"]),
								trrs_id = Convert.ToInt32(dr["trrs_id"]),

								approved_by = dr["approved_by"]?.ToString(),

								approved_date =
									dr["approved_date"] == DBNull.Value
									? null
									: Convert.ToDateTime(dr["approved_date"]),

								approver_remarks =
									dr["approver_remarks"]?.ToString(),

								approver_part =
									Convert.ToInt32(dr["approver_part"]),

								approver_role =
									dr["approver_role"]?.ToString(),

								done_approve =
									Convert.ToBoolean(dr["done_approve"]),

								for_approve =
									dr["for_approve"] == DBNull.Value
									? null
									: Convert.ToInt32(dr["for_approve"]),

								dept_id =
									dr["dept_id"] == DBNull.Value
									? null
									: Convert.ToInt32(dr["dept_id"]),

								parent_dept_id =
									dr["parent_dept_id"] == DBNull.Value
									? null
									: Convert.ToInt32(dr["parent_dept_id"]),

								section_id =
									dr["section_id"] == DBNull.Value
									? null
									: Convert.ToInt32(dr["section_id"])	,
								has_attachment 	= Convert.ToBoolean(dr["has_attachment"])
							});
						}
					}
				}
			}

			return list;
		}

		public List<ApproverAttachmentModel> GetApproverAttachments(int trrsId,int approvingPart)
		{
			List<ApproverAttachmentModel> attachments = new();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand(
				"trrs_sp_get_approver_attachments",
				conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@trrs_id", trrsId);
				cmd.Parameters.AddWithValue("@approving_part", approvingPart);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						attachments.Add(new ApproverAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							approving_part = Convert.ToInt32(reader["approving_part"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						});
					}
				}
			}

			return attachments;
		}

		public List<RootCauseAttachmentModel> GetRootCauseAttachments(int trrsId)
		{
			List<RootCauseAttachmentModel> attachments = new();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_root_cause_attachments", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@trrs_id", trrsId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						attachments.Add(new RootCauseAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						});
					}
				}
			}

			return attachments;
		}

		public List<MomAttachmentModel> GetMomAttachments(int trrsId)
		{
			List<MomAttachmentModel> attachments = new();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_mom_attachment_list", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@trrs_id", trrsId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						attachments.Add(new MomAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						});
					}
				}
			}

			return attachments;
		}

		public List<ActionPlanAttachmentModel> GetActionPlanAttachments(int trrsId)
		{
			List<ActionPlanAttachmentModel> attachments = new();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_action_plan_attachment_list", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@trrs_id", trrsId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						attachments.Add(new ActionPlanAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						});
					}
				}
			}

			return attachments;
		}

		public RootCauseAttachmentModel GetRootCauseAttachment(int attachmentId)
		{
			RootCauseAttachmentModel attachment = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_root_cause_attachment_file", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@attachment_id", attachmentId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						attachment = new RootCauseAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						};
					}
				}
			}

			return attachment;
		}

		public ActionPlanAttachmentModel GetActionPlanAttachment(int attachmentId)
		{
			ActionPlanAttachmentModel attachment = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_action_plan_attachment_file", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@attachment_id", attachmentId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						attachment = new ActionPlanAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						};
					}
				}
			}

			return attachment;
		}


		public MomAttachmentModel GetMomAttachment(int attachmentId)
		{
			MomAttachmentModel attachment = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_Mom_attachment_file", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@attachment_id", attachmentId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						attachment = new MomAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						};
					}
				}
			}

			return attachment;
		}

		public ApproverAttachmentModel GetApproverAttachmentById(int attachmentId)
		{
			ApproverAttachmentModel attachment = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_approver_attachment_by_id", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@attachment_id", attachmentId);

				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						attachment = new ApproverAttachmentModel
						{
							attachment_id = Convert.ToInt32(reader["attachment_id"]),
							trrs_id = Convert.ToInt32(reader["trrs_id"]),
							approving_part = Convert.ToInt32(reader["approving_part"]),
							file_name = reader["file_name"]?.ToString(),
							file_data = reader["file_data"] as byte[]
						};
					}
				}
			}

			return attachment;
		}


		public ProductStatusModel GetProductStatus(int trrsId)
		{
			ProductStatusModel model = null;

			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_product_status", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@trrs_id", trrsId);

				con.Open();

				using (SqlDataReader dr = cmd.ExecuteReader())
				{
					if (dr.Read())
					{
						model = new ProductStatusModel
						{
							product_status_id = Convert.ToInt32(dr["product_status_id"]),
							trrs_id = Convert.ToInt32(dr["trrs_id"]),
							is_rush = Convert.ToBoolean(dr["is_rush"]),
							is_next_production = Convert.ToBoolean(dr["is_next_production"]),
							status_as_of = dr["status_as_of"] == DBNull.Value
								? null
								: Convert.ToDateTime(dr["status_as_of"]),
							required_date = dr["required_date"] == DBNull.Value
								? null
								: Convert.ToDateTime(dr["required_date"]),
							status_details = dr["status_details"]?.ToString()
						};
					}
				}
			}

			return model;
		}


		public async Task<ActualActivityInfoModel?> GetActualActivityInfo(int trrsId)
		{
			ActualActivityInfoModel? model = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_get_actual_activity_info", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", trrsId);

					await conn.OpenAsync();

					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							model = new ActualActivityInfoModel
							{
								trrs_id = Convert.ToInt32(reader["trrsid"]),

								datereceived = reader["datereceived"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["datereceived"]),

								dateaccomplishedchange = reader["dateaccomplishedchange"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["dateaccomplishedchange"]),

								designchangedetails = reader["designchangedetails"]?.ToString(),

								partmodifieddetails = reader["partmodifieddetails"]?.ToString(),

								moldresult = reader["moldresult"]?.ToString(),

								dateaccomplishedmodified = reader["dateaccomplishedmodified"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["dateaccomplishedmodified"]),

								dateaccomplishedresult = reader["dateaccomplishedresult"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["dateaccomplishedresult"])
							};
						}
					}
				}
			}

			return model;
		}

		public async Task<ProductionScheduleModel?> GetProductionSchedule(int trrsId)
		{
			ProductionScheduleModel? model = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_get_production_schedule", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", trrsId);

					await conn.OpenAsync();

					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							model = new ProductionScheduleModel
							{
								sched_id = Convert.ToInt32(reader["schedid"]),
								trrs_id = Convert.ToInt32(reader["trrsid"]),

								proddatereceived = reader["datereceived"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["datereceived"]),

								prodtesting = reader["datetesting"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["datetesting"])
							};
						}
					}
				}
			}

			return model;
		}


		public async Task<EvaluationResultModel> GetEvaluationResultAsync(int trrsId)
		{
			EvaluationResultModel model = null;

			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_get_evaluation_result", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@trrs_id", trrsId);

				await con.OpenAsync();

				using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
				{
					if (await dr.ReadAsync())
					{
						model = new EvaluationResultModel
						{
							evaluation_id = Convert.ToInt32(dr["evaluation_id"]),
							trrs_id = Convert.ToInt32(dr["trrs_id"]),
							productresult = dr["product_result"]?.ToString(),
							isapproved = Convert.ToBoolean(dr["is_approved"])
						};
					}
				}
			}

			return model;
		}


		public async Task<QualityVerificationModel?> GetQualityVerification(int trrsId)
		{
			QualityVerificationModel? model = null;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_get_quality_verification", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", trrsId);

					await conn.OpenAsync();

					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							model = new QualityVerificationModel
							{
								verification_id = Convert.ToInt32(reader["verification_id"]),
								trrs_id = Convert.ToInt32(reader["trrs_id"]),

								job_order = reader["job_order"] == DBNull.Value
									? null
									: reader["job_order"].ToString()
							};
						}
					}
				}
			}

			return model;
		}



		public async Task<List<DeviationDetailModel>> GetDeviationDetails(int trrsId, int? deviationId = null)
		{
			var list = new List<DeviationDetailModel>();

			using (SqlConnection con = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_get_deviation_details", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrsid", trrsId);

					if (deviationId.HasValue)
						cmd.Parameters.AddWithValue("@deviationid", deviationId.Value);
					else
						cmd.Parameters.AddWithValue("@deviationid", DBNull.Value);

					await con.OpenAsync();

					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							list.Add(new DeviationDetailModel
							{
								deviation_detail_id = Convert.ToInt32(reader["deviation_detail_id"]),
								deviation_id = Convert.ToInt32(reader["deviation_id"]),
								trrs_id = Convert.ToInt32(reader["trrs_id"]),
								deviation_details = reader["deviation_details"]?.ToString() ?? string.Empty,
								deviation_date = reader["deviation_date"] == DBNull.Value
									? null
									: Convert.ToDateTime(reader["deviation_date"]),
								deviation_version = Convert.ToInt32(reader["deviation_version"])
							});
						}
					}
				}
			}

			return list;
		}


		//Dashboard


		public async Task<DataTable> GetDashboardSummaryAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_dashboard_summary", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetMonthlyRequestsAsync(int year)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_monthly_requests", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", year);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetRequestsByTypeAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_requests_by_type", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetRequestsByCustomerAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_requests_by_customer", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetRequestsByProductAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_requests_by_product", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}

		public async Task<DataTable> GetRevisionDistributionAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_revision_distribution", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetRequestsByMoldAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_requests_by_mold", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetParentChildSummaryAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_parent_child_summary", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}

		public async Task<DataTable> GetTopRequestedProductsAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_top_requested_products", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetDetailedBreakdownAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_detailed_breakdown", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			using SqlDataAdapter da = new(cmd);

			await con.OpenAsync();
			da.Fill(dt);

			return dt;
		}

		public async Task<DataTable> GetMonthlyTrendAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_rpt_monthly_trend", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			using SqlDataAdapter da = new(cmd);

			await con.OpenAsync();
			da.Fill(dt);

			return dt;
		}



		public async Task<List<TrrsExportModel>> GetRequestListReportAsync()
		{
			var requests = new List<TrrsExportModel>();

			using var con = new SqlConnection(_connectionString);
			using var cmd = new SqlCommand("trrs_sp_get_request_list", con);

			cmd.CommandType = CommandType.StoredProcedure;

			await con.OpenAsync();

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				requests.Add(new TrrsExportModel
				{
					ControlNo = reader["controlno"]?.ToString(),
					TRRFNo = reader["trrfno"]?.ToString(),
					Customer = reader["customername"]?.ToString(),
					Type = reader["trrstype"]?.ToString(),
					Product = reader["productname"]?.ToString(),
					MoldNo = reader["moldno"] != DBNull.Value
								? Convert.ToInt32(reader["moldno"])
								: 0,
					PartModelNo = reader["partmodelno"]?.ToString(),
					MachineName = reader["machinename"]?.ToString(),
					Problem = reader["problem"]?.ToString(),
					SubmittedBy = reader["submittedby"]?.ToString(),
					DatePrepared = reader["dateprepared"] != DBNull.Value
								? Convert.ToDateTime(reader["dateprepared"])
								: DateTime.MinValue,
					CurrentStatus = reader["task"]?.ToString(),
					CurrentPart = reader["currentpart"]?.ToString(),
					RevisionNo = reader["trrsversion"] != DBNull.Value
								? Convert.ToInt32(reader["trrsversion"])
								: 0

				});
			}

			return requests;
		}


		public async Task<DataTable> GetApprovalsHorizontalAsync(int? trrsId = null)
		{
			DataTable dt = new DataTable();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_get_approvals_horizontal", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Add("@trrs_id", SqlDbType.Int).Value =
						trrsId.HasValue ? trrsId.Value : DBNull.Value;

					await conn.OpenAsync();

					using (SqlDataAdapter da = new SqlDataAdapter(cmd))
					{
						da.Fill(dt);
					}
				}
			}

			return dt;
		}


		public async Task<List<ProductLookupModel>> GetProductsByCustomerAsync(int customerId)
		{
			List<ProductLookupModel> products = new();

			try
			{
				using SqlConnection conn = new SqlConnection(_connectionString);
				using SqlCommand cmd = new SqlCommand("trrs_sp_get_products_by_customer", conn);

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@customer_id", customerId);

				await conn.OpenAsync();

				using SqlDataReader reader = await cmd.ExecuteReaderAsync();

				while (await reader.ReadAsync())
				{
					products.Add(new ProductLookupModel
					{
						productId = Convert.ToInt32(reader["ProductId"]),
						productName = reader["ProductName"]?.ToString() ?? string.Empty,
						gml = reader["GML"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GML"]),
						modelpartno = reader["ModelDescription"] == DBNull.Value
							? null
							: reader["ModelDescription"].ToString(),
						customerid = reader["CustomerId"] == DBNull.Value
							? null
							: Convert.ToInt32(reader["CustomerId"])
					});
				}

				return products;
			}
			catch (Exception ex)
			{
				// During development
				throw new Exception($"GetProductsByCustomerAsync failed. {ex.Message}", ex);
			}
		}


		public async Task<DataTable> GetTRRFDistributionByLifeUtilizationAsync(int? year, int? month)
		{
			var dt = new DataTable();

			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("sp_GetTRRFDistributionByLifeUtilization", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

			await con.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			dt.Load(reader);

			return dt;
		}


		public async Task<DataTable> GetTRRFDetailsByStageIdAsync(int stageId, int? year, int? month)
		{
			var dt = new DataTable();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("sp_GetTRRFDetailsByStageId", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@StageId", stageId);
				cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);

				await conn.OpenAsync();

				using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{
					dt.Load(reader);
				}
			}

			return dt;
		}


		public async Task<AIRequestContext?> GetAIRequestContextAsync(int trrsId)
		{
			using SqlConnection conn = new(_connectionString);

			using SqlCommand cmd = new(
				"trrs_sp_ai_get_request_context",
				conn);

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@trrs_id", trrsId);

			await conn.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			AIRequestContext context = new();

			//----------------------------------------------------------
			// Result Set 1 - Product Info
			//----------------------------------------------------------
			if (await reader.ReadAsync())
			{
				context.ProductInfo = new AIProductInfo
				{
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					TrrfNo = reader["trrf_no"]?.ToString(),
					TrrsType = reader["trrs_type"]?.ToString(),
					TrrsVersion = Convert.ToInt32(reader["trrs_version"]),
					TrrsParentId	 = reader["trrs_parent_id"] == DBNull.Value ? null : Convert.ToInt32(reader["trrs_parent_id"]),
					DatePrepared = Convert.ToDateTime(reader["date_prepared"]),
					CreateUserId = reader["create_user_id"]?.ToString(),
					CreateUserName = reader["create_user_name"]?.ToString(),

					CustomerId = Convert.ToInt32(reader["customer_id"]),
					CustomerCode = reader["customer_code"]?.ToString(),
					CustomerName = reader["customer_name"]?.ToString(),

					ProductId = Convert.ToInt32(reader["product_id"]),
					ProductName = reader["product_name"]?.ToString(),

					MoldNo = Convert.ToInt32(reader["mold_no"]),
					PartModelNo = reader["part_model_no"]?.ToString(),

					DeviationId = reader["deviation_id"] == DBNull.Value ? null : Convert.ToInt32(reader["deviation_id"]),
					IsDeleted = Convert.ToBoolean(reader["is_deleted"])
				};
			}

			//----------------------------------------------------------
			// Result Set 2 - Problem Details
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.ProblemDetails = new AIProblemDetails
				{
					ProblemId = Convert.ToInt32(reader["problem_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					EncounteredProblem = reader["encountered_problem"]?.ToString(),
					MachineName = reader["machine_name"]?.ToString(),
					MoldToolLife = Convert.ToInt32(reader["mold_tool_life"]),

					RcWearTear = Convert.ToBoolean(reader["rc_wear_tear"]),
					RcMachineError = Convert.ToBoolean(reader["rc_machine_error"]),
					RcDesignError = Convert.ToBoolean(reader["rc_design_error"]),
					RcFabricationError = Convert.ToBoolean(reader["rc_fabrication_error"]),
					RcEffectOfPrevImprovement = Convert.ToBoolean(reader["rc_effect_of_prev_improvement"]),
					RcCustomerRequirement = Convert.ToBoolean(reader["rc_customer_requirement"]),
					RcDetails = reader["rc_details"]?.ToString(),

					ApRepair = Convert.ToBoolean(reader["ap_repair"]),
					ApAdjustment = Convert.ToBoolean(reader["ap_adjustment"]),
					ApRevision = Convert.ToBoolean(reader["ap_revision"]),
					ApReplacement = Convert.ToBoolean(reader["ap_replacement"]),
					ApTrialTesting = Convert.ToBoolean(reader["ap_trial_testing"]),
					ApDetails = reader["ap_details"]?.ToString(),

					HasSpare = Convert.ToBoolean(reader["has_spare"]),
					Quantity = Convert.ToInt32(reader["quantity"])
				};
			}

			//----------------------------------------------------------
			// Result Set 3 - Product Status
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.ProductStatus = new AIProductStatus
				{
					ProductStatusId = Convert.ToInt32(reader["product_status_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					IsRush = Convert.ToBoolean(reader["is_rush"]),
					IsNextProduction = Convert.ToBoolean(reader["is_next_production"]),
					StatusAsOf = reader["status_as_of"] == DBNull.Value ? null : Convert.ToDateTime(reader["status_as_of"]),
					RequiredDate = reader["required_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["required_date"]),
					StatusDetails = reader["status_details"]?.ToString()
				};
			}

			//----------------------------------------------------------
			// Result Set 4 - Evaluation
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.EvaluationResult = new AIEvaluationResult
				{
					EvaluationId = Convert.ToInt32(reader["evaluation_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					ProductResult = reader["product_result"]?.ToString(),
					IsApproved = Convert.ToBoolean(reader["is_approved"])
				};
			}

			//----------------------------------------------------------
			// Result Set 5 - Production Schedule
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.ProductionSchedule = new AIProductionSchedule
				{
					SchedId = Convert.ToInt32(reader["sched_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					DateReceived = reader["date_received"] == DBNull.Value ? null : Convert.ToDateTime(reader["date_received"]),
					DateTesting = reader["date_testing"] == DBNull.Value ? null : Convert.ToDateTime(reader["date_testing"])
				};
			}

			//----------------------------------------------------------
			// Result Set 6 - Actual Activity
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.ActualActivity = new AIActualActivity
				{
					ActivityId = Convert.ToInt32(reader["activity_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					DateReceived = reader["date_received"] == DBNull.Value ? null : Convert.ToDateTime(reader["date_received"]),
					DateAccomplishedChange = reader["date_accomplished_change"] == DBNull.Value ? null : Convert.ToDateTime(reader["date_accomplished_change"]),
					DesignChangeDetails = reader["design_change_details"]?.ToString(),
					PartModifiedDetails = reader["part_modified_details"]?.ToString(),
					MoldResult = reader["mold_result"]?.ToString(),
					DateAccomplishedModified = reader["date_accomplished_modified"] == DBNull.Value ? null : Convert.ToDateTime(reader["date_accomplished_modified"]),
					DateAccomplishedResult = reader["date_accomplished_result"] == DBNull.Value ? null : Convert.ToDateTime(reader["date_accomplished_result"])
				};
			}

			//----------------------------------------------------------
			// Result Set 7 - Quality Verification
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.QualityVerification = new AIQualityVerification
				{
					VerificationId = Convert.ToInt32(reader["verification_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					JobOrder = reader["job_order"]?.ToString()
				};
			}

			//----------------------------------------------------------
			// Result Set 8 - Approval History
			//----------------------------------------------------------
			await reader.NextResultAsync();

			while (await reader.ReadAsync())
			{
				context.ApprovalHistory.Add(new AIApprovalHistory
				{
					ApprovalId = Convert.ToInt32(reader["approval_id"]),
					ApproverRole = reader["approver_role"]?.ToString(),
					ApproverPart = Convert.ToInt32(reader["approver_part"]),
					ApprovedBy = reader["approved_by"]?.ToString(),
					ApprovedDate = reader["approved_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["approved_date"]),
					ApproverRemarks = reader["approver_remarks"]?.ToString(),
					DoneApprove = Convert.ToInt32(reader["done_approve"]),
					ForApprove = Convert.ToInt32(reader["for_approve"]),
					DeptId = Convert.ToInt32(reader["dept_id"]),
					ParentDeptId = Convert.ToInt32(reader["parent_dept_id"]),
					SectionId = Convert.ToInt32(reader["section_id"])
				});
			}

			//----------------------------------------------------------
			// Result Set 9 - Life Utilization
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.LifeUtilization = new AILifeUtilization
				{
					DataId = Convert.ToInt32(reader["data_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					ConditionStageId = Convert.ToInt32(reader["condition_stage_id"]),
					UtilizationPercentage = Convert.ToDecimal(reader["utilization_percentage"]),
					ComputedShots = Convert.ToInt32(reader["computed_shots"]),
					GuaranteedLife = Convert.ToInt32(reader["guaranteed_life"]),
					RemainingShots = Convert.ToInt32(reader["remaining_shots"]),
					CreatedDate = Convert.ToDateTime(reader["created_date"]),
					CreatedBy = reader["created_by"]?.ToString(),

					ConditionStage = reader["condition_stage"]?.ToString(),
					PercentageMin = Convert.ToDecimal(reader["percentage_min"]),
					PercentageMax = reader["percentage_max"] == DBNull.Value ? null : Convert.ToDecimal(reader["percentage_max"])
				};
			}

			//----------------------------------------------------------
			// Result Set 10 - Deviation
			//----------------------------------------------------------
			await reader.NextResultAsync();

			if (await reader.ReadAsync())
			{
				context.Deviation = new AIDeviation
				{
					DeviationId = Convert.ToInt32(reader["deviation_id"]),
					TrrsId = Convert.ToInt32(reader["trrs_id"]),
					WithDeviation = Convert.ToBoolean(reader["with_deviation"]),
					DeviationCount = Convert.ToInt32(reader["deviation_count"])
				};
			}

			//----------------------------------------------------------
			// Result Set 11 - Deviation Details
			//----------------------------------------------------------
			await reader.NextResultAsync();

			while (await reader.ReadAsync())
			{
				context.DeviationDetails.Add(new AIDeviationDetail
				{
					DeviationDetailId = Convert.ToInt32(reader["deviation_detail_id"]),
					DeviationId = Convert.ToInt32(reader["deviation_id"]),
					DeviationDetails = reader["deviation_details"]?.ToString(),
					DeviationDate = reader["deviation_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["deviation_date"]),
					DeviationVersion = Convert.ToInt32(reader["deviation_version"])
				});
			}

			//----------------------------------------------------------
			// Result Set 12 - Attachments
			//----------------------------------------------------------
			await reader.NextResultAsync();

			while (await reader.ReadAsync())
			{
				context.Attachments.Add(new AIAttachment
				{
					AttachmentType = reader["attachment_type"]?.ToString(),
					FileName = reader["file_name"]?.ToString(),
					Remarks = reader["remarks"]?.ToString()
				});
			}

			return context;
		}


		public async Task<List<AISearchRequestContext>> GetAISearchRequestContextAsync()
		{
			var list = new List<AISearchRequestContext>();

			using SqlConnection conn = new(_connectionString);

			using SqlCommand cmd = new(
				"trrs_sp_ai_search_requests",
				conn);

			cmd.CommandType = CommandType.StoredProcedure;

			await conn.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();


			while (await reader.ReadAsync())
			{
				list.Add(new AISearchRequestContext
				{
					// ------------------------------------------------------------
					// TRRF INFORMATION
					// ------------------------------------------------------------

					TrrfId = Convert.ToInt32(reader["TRRF_ID"]),

					TrrfCode = reader["TRRF_Code"]?.ToString(),

					RequestType = reader["Request_Type"]?.ToString(),

					RequestVersion =
						reader["Request_Version"] == DBNull.Value
						? 0
						: Convert.ToInt32(reader["Request_Version"]),


					DatePrepared =
						reader["Date_Prepared"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Date_Prepared"]),


					TicketStatus = reader["Ticket_Status"]?.ToString(),
					CurrentPart = reader["Current_Part"] == DBNull.Value ? null : Convert.ToInt32(reader["Current_Part"]),
					DeviationId = reader["Deviation_ID"] == DBNull.Value ? null : Convert.ToInt32(reader["Deviation_ID"]),
					HasRevisionOrDeviation = reader["Has_Revision_Or_Deviation"] == DBNull.Value ? null : Convert.ToBoolean(reader["Has_Revision_Or_Deviation"]),
					AttemptNumber = reader["Attempt_Number"] == DBNull.Value ? null : Convert.ToInt32(reader["Attempt_Number"]),


					// ------------------------------------------------------------
					// REQUESTER
					// ------------------------------------------------------------

					PreparedBy = reader["Prepared_By"]?.ToString(),



					// ------------------------------------------------------------
					// CUSTOMER
					// ------------------------------------------------------------

					CustomerCode = reader["Customer_Code"]?.ToString(),

					CustomerName = reader["Customer_Name"]?.ToString(),



					// ------------------------------------------------------------
					// PRODUCT / MOLD
					// ------------------------------------------------------------

					ProductName = reader["Product_Name"]?.ToString(),

					MoldNumber =
						reader["Mold_Number"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Mold_Number"]),


					PartModelNumber =
						reader["Part_Model_Number"]?.ToString(),



					// ------------------------------------------------------------
					// PROBLEM
					// ------------------------------------------------------------

					ProblemDescription =
						reader["Problem_Description"]?.ToString(),


					MachineName =
						reader["Machine_Name"]?.ToString(),


					CurrentMoldToolLife =
						reader["Current_Mold_Tool_Life"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Current_Mold_Tool_Life"]),



					// ------------------------------------------------------------
					// ROOT CAUSE
					// ------------------------------------------------------------

					RootCauseWearTear =
						reader["RootCause_Wear_Tear"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["RootCause_Wear_Tear"]),


					RootCauseMachineError =
						reader["RootCause_Machine_Error"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["RootCause_Machine_Error"]),


					RootCauseDesignError =
						reader["RootCause_Design_Error"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["RootCause_Design_Error"]),


					RootCauseFabricationError =
						reader["RootCause_Fabrication_Error"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["RootCause_Fabrication_Error"]),


					RootCausePreviousImprovement =
						reader["RootCause_Previous_Improvement"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["RootCause_Previous_Improvement"]),


					RootCauseCustomerRequirement =
						reader["RootCause_Customer_Requirement"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["RootCause_Customer_Requirement"]),


					RootCauseDetails =
						reader["RootCause_Details"]?.ToString(),



					// ------------------------------------------------------------
					// ACTION PLAN
					// ------------------------------------------------------------

					ActionRepair =
						reader["Action_Repair"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Action_Repair"]),


					ActionAdjustment =
						reader["Action_Adjustment"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Action_Adjustment"]),


					ActionRevision =
						reader["Action_Revision"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Action_Revision"]),


					ActionReplacement =
						reader["Action_Replacement"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Action_Replacement"]),


					ActionTrialTesting =
						reader["Action_Trial_Testing"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Action_Trial_Testing"]),


					ActionPlanDetails =
						reader["Action_Plan_Details"]?.ToString(),



					// ------------------------------------------------------------
					// PRODUCT STATUS
					// ------------------------------------------------------------

					IsRushRequest =
						reader["Is_Rush_Request"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Is_Rush_Request"]),


					IsNextProduction =
						reader["Is_Next_Production"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Is_Next_Production"]),


					StatusDate =
						reader["Status_Date"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Status_Date"]),


					RequiredDate =
						reader["Required_Date"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Required_Date"]),


					ProductStatusRemarks =
						reader["Product_Status_Remarks"]?.ToString(),



					// ------------------------------------------------------------
					// EVALUATION
					// ------------------------------------------------------------

					EvaluationResult =
						reader["Evaluation_Result"]?.ToString(),


					EvaluationApproved =
						reader["Evaluation_Approved"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Evaluation_Approved"]),



					// ------------------------------------------------------------
					// PRODUCTION
					// ------------------------------------------------------------

					ProductionDateReceived =
						reader["Production_Date_Received"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Production_Date_Received"]),


					ProductionTestingDate =
						reader["Production_Testing_Date"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Production_Testing_Date"]),



					// ------------------------------------------------------------
					// ACTUAL ACTIVITY
					// ------------------------------------------------------------

					ChangeCompletedDate =
						reader["Change_Completed_Date"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Change_Completed_Date"]),


					DesignChangeDetails =
						reader["Design_Change_Details"]?.ToString(),


					PartModificationDetails =
						reader["Part_Modification_Details"]?.ToString(),


					MoldResult =
						reader["Mold_Result"]?.ToString(),


					ModificationCompletedDate =
						reader["Modification_Completed_Date"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Modification_Completed_Date"]),


					ResultCompletedDate =
						reader["Result_Completed_Date"] == DBNull.Value
						? null
						: Convert.ToDateTime(reader["Result_Completed_Date"]),



					// ------------------------------------------------------------
					// QUALITY
					// ------------------------------------------------------------

					QualityJobOrder =
						reader["Quality_Job_Order"]?.ToString(),



					// ------------------------------------------------------------
					// MOLD LIFE
					// ------------------------------------------------------------

					MoldLifeStage =
						reader["Mold_Life_Stage"]?.ToString(),


					MoldLifeUtilizationPercentage =
						reader["Mold_Life_Utilization_Percentage"] == DBNull.Value
						? null
						: Convert.ToDecimal(reader["Mold_Life_Utilization_Percentage"]),


					CurrentShotCount =
						reader["Current_Shot_Count"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Current_Shot_Count"]),


					GuaranteedMoldLife =
						reader["Guaranteed_Mold_Life"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Guaranteed_Mold_Life"]),


					RemainingMoldLife =
						reader["Remaining_Mold_Life"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Remaining_Mold_Life"]),



					// ------------------------------------------------------------
					// DEVIATION
					// ------------------------------------------------------------

					HasDeviation =
						reader["Has_Deviation"] == DBNull.Value
						? null
						: Convert.ToBoolean(reader["Has_Deviation"]),


					DeviationCount =
						reader["Deviation_Count"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Deviation_Count"]),


					DeviationDetails =
						reader["Deviation_Details"]?.ToString(),


					DeviationVersion =
						reader["Deviation_Version"] == DBNull.Value
						? null
						: Convert.ToInt32(reader["Deviation_Version"])

				});
			}


			return list;
		}


	}
}
