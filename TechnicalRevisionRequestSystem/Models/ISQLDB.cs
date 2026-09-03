using Microsoft.Data.SqlClient;
using System.Data;
using TechnicalRevisionRequestSystem.Models;

namespace TechnicalRevisionRequestSystem.Models
{
	public class ISQLDB : TRRSRepositoryInsert
	{

		private readonly string _connectionString;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ISQLDB(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = configuration.GetConnectionString("TRRSConnection");
			_httpContextAccessor = httpContextAccessor;
		}


		public async Task<int> InsertRequestAsync(InsertTrrsRequestModel model)
		{
			try
			{
				using var con = new SqlConnection(_connectionString);
				using var cmd = new SqlCommand("[dbo].[trrs_sp_insert_request]", con);

				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@type", model.Type);
				cmd.Parameters.AddWithValue("@create_user", model.CreateUser);
				cmd.Parameters.AddWithValue("@dept_id", model.DeptId);
				cmd.Parameters.AddWithValue("@section_id", model.SectionId);

				cmd.Parameters.AddWithValue("@customer_id", model.CustomerId);
				cmd.Parameters.AddWithValue("@product_id", model.ProductId);
				cmd.Parameters.AddWithValue("@mold_no", model.MoldNo);
				cmd.Parameters.AddWithValue("@part_model_no", model.PartModelNo);
				cmd.Parameters.AddWithValue("@date_prepared", model.DatePrepared);

				cmd.Parameters.AddWithValue("@create_user_id", model.CreateUserId);
				cmd.Parameters.AddWithValue("@create_user_name", model.CreateUserName);

				cmd.Parameters.AddWithValue("@encountered_problem", model.EncounteredProblem);
				cmd.Parameters.AddWithValue("@machine_name", model.MachineName);
				cmd.Parameters.AddWithValue("@mold_tool_life", model.MoldToolLife);

				cmd.Parameters.AddWithValue("@rc_wear_tear", model.RcWearTear);
				cmd.Parameters.AddWithValue("@rc_machine_error", model.RcMachineError);
				cmd.Parameters.AddWithValue("@rc_design_error", model.RcDesignError);
				cmd.Parameters.AddWithValue("@rc_fabrication_error", model.RcFabricationError);
				cmd.Parameters.AddWithValue("@rc_effect_of_prev_improvement", model.RcEffectOfPrevImprovement);
				cmd.Parameters.AddWithValue("@rc_customer_requirement", model.RcCustomerRequirement);
				cmd.Parameters.AddWithValue("@rc_details", model.RcDetails);

				cmd.Parameters.AddWithValue("@ap_repair", model.ApRepair);
				cmd.Parameters.AddWithValue("@ap_adjustment", model.ApAdjustment);
				cmd.Parameters.AddWithValue("@ap_revision", model.ApRevision);
				cmd.Parameters.AddWithValue("@ap_replacement", model.ApReplacement);
				cmd.Parameters.AddWithValue("@ap_trial_testing", model.ApTrialTesting);
				cmd.Parameters.AddWithValue("@ap_details", model.ApDetails);

				cmd.Parameters.AddWithValue("@has_spare", model.HasSpare);
				cmd.Parameters.AddWithValue("@quantity", model.Quantity);
				cmd.Parameters.AddWithValue("@productname", model.productname);

				await con.OpenAsync();

				var result = await cmd.ExecuteScalarAsync();

				if (result == null || result == DBNull.Value)
				{
					throw new Exception("Stored procedure did not return a TRRS ID.");
				}

				return Convert.ToInt32(result);
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to insert request: {ex.Message}", ex);
			}
		}


		public async Task<int> InsertRootCauseAttachmentAsync(int trrsId,string fileName,byte[] fileData)
		{
			using var con = new SqlConnection(_connectionString);

			using var cmd = new SqlCommand(
				"[dbo].[trrs_sp_insert_root_cause_attachment]",
				con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@trrs_id", trrsId);
			cmd.Parameters.AddWithValue("@file_name", fileName);
			cmd.Parameters.AddWithValue("@file_data", fileData);

			await con.OpenAsync();

			var result = await cmd.ExecuteScalarAsync();

			return Convert.ToInt32(result);
		}

		public async Task<int> InsertActionPlanAttachmentAsync(int trrsId,string fileName,byte[] fileData)
		{
			using var con = new SqlConnection(_connectionString);

			using var cmd = new SqlCommand(
				"[dbo].[trrs_sp_insert_action_plan_attachment]",
				con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@trrs_id", trrsId);
			cmd.Parameters.AddWithValue("@file_name", fileName);
			cmd.Parameters.AddWithValue("@file_data", fileData);

			await con.OpenAsync();

			var result = await cmd.ExecuteScalarAsync();

			return Convert.ToInt32(result);
		}

		public async Task<int> InsertMomAttachmentAsync(int trrsId,string fileName,byte[] fileData,string? remarks)
		{
			using SqlConnection con = new(_connectionString);
			using SqlCommand cmd = new("trrs_sp_insert_mom_attachment", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@TrrsId", trrsId);
			cmd.Parameters.AddWithValue("@FileName", fileName);
			cmd.Parameters.AddWithValue("@FileData", fileData);
			cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);

			await con.OpenAsync();

			object? result = await cmd.ExecuteScalarAsync();

			return result != null
				? Convert.ToInt32(result)
				: 0;
		}


		public async Task<int> InsertApprovalAttachmentAsync(int trrsId,int approvingPart,string fileName,byte[] fileData)
		{
			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_insert_attachment_others", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@trrs_id", SqlDbType.Int).Value = trrsId;
				cmd.Parameters.Add("@approving_part", SqlDbType.Int).Value = approvingPart;
				cmd.Parameters.Add("@file_name", SqlDbType.NVarChar, 255).Value = fileName;
				cmd.Parameters.Add("@file_data", SqlDbType.VarBinary).Value = fileData;

				await con.OpenAsync();

				object result = await cmd.ExecuteScalarAsync();

				return Convert.ToInt32(result);
			}
		}


		public async Task<bool> SaveProductStatus(ProductStatusModel model)
		{
			using SqlConnection con = new SqlConnection(_connectionString);

			using SqlCommand cmd = new SqlCommand(
				"trrs_sp_save_product_status",
				con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);
			cmd.Parameters.AddWithValue("@is_rush", model.is_rush);
			cmd.Parameters.AddWithValue("@is_next_production", model.is_next_production);
			cmd.Parameters.AddWithValue("@status_as_of",
				(object?)model.status_as_of ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@required_date",
				(object?)model.required_date ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@status_details",
				(object?)model.status_details ?? DBNull.Value);

			await con.OpenAsync();

			await cmd.ExecuteNonQueryAsync();

			return true;
		}


		public async Task<int> InsertActualActivityInfo(ActualActivityInfoModel model)
		{
			int activityId = 0;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_insert_actual_activity_info", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);

					cmd.Parameters.AddWithValue(
						"@design_change_details",
						(object?)model.designchangedetails ?? DBNull.Value);

					cmd.Parameters.AddWithValue(
						"@date_accomplished_change",
						(object?)model.dateaccomplishedchange ?? DBNull.Value);

					cmd.Parameters.AddWithValue(
						"@date_received",
						(object?)model.datereceived ?? DBNull.Value);

					await conn.OpenAsync();

					object? result = await cmd.ExecuteScalarAsync();

					if (result != null && result != DBNull.Value)
					{
						activityId = Convert.ToInt32(result);
					}
				}
			}

			return activityId;
		}


		public async Task<int> InsertProductionSchedule(ProductionScheduleModel model)
		{
			int schedId = 0;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_insert_production_schedule", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);

					cmd.Parameters.AddWithValue(
						"@date_received",
						(object?)model.proddatereceived ?? DBNull.Value);
					cmd.Parameters.AddWithValue(
						"@date_testing",
						(object?)model.prodtesting ?? DBNull.Value);

					await conn.OpenAsync();

					object? result = await cmd.ExecuteScalarAsync();

					if (result != null && result != DBNull.Value)
					{
						schedId = Convert.ToInt32(result);
					}
				}
			}

			return schedId;
		}



		public async Task<bool> SaveEvaluationResult(EvaluationResultModel model)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(_connectionString))
				{
					using (SqlCommand cmd = new SqlCommand("trrs_sp_save_evaluation_result", con))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);

						cmd.Parameters.AddWithValue(
							"@product_result",
							string.IsNullOrWhiteSpace(model.productresult)
								? (object)DBNull.Value
								: model.productresult);

						cmd.Parameters.AddWithValue("@is_approved", model.isapproved);
						await con.OpenAsync();

						await cmd.ExecuteNonQueryAsync();

						return true;
					}
				}
			}
			catch
			{
				throw;
			}
		}


		public async Task<bool> SaveQualityVerification(QualityVerificationModel model)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					using (SqlCommand cmd = new SqlCommand("trrs_sp_save_quality_verification", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);

						cmd.Parameters.AddWithValue(
							"@job_order",
							string.IsNullOrWhiteSpace(model.job_order)
								? (object)DBNull.Value
								: model.job_order);

						await conn.OpenAsync();

						await cmd.ExecuteNonQueryAsync();

						return true;
					}
				}
			}
			catch
			{
				throw;
			}
		}


		public async Task<int> InsertDeviationDetail(DeviationDetailModel model)
		{
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_insert_deviation_detail", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@deviationid", model.deviation_id);

					cmd.Parameters.AddWithValue(
						"@deviationdetails",
						string.IsNullOrWhiteSpace(model.deviation_details)
							? (object)DBNull.Value
							: model.deviation_details);

					cmd.Parameters.AddWithValue(
						"@deviationdate",
						model.deviation_date.HasValue
							? model.deviation_date.Value
							: (object)DBNull.Value);

					await conn.OpenAsync();

					object result = await cmd.ExecuteScalarAsync();

					return Convert.ToInt32(result);
				}
			}
		}


		public async Task<CustomerInsertResult> InsertCustomerAsync(string customerCode,string customerName)
		{
			CustomerInsertResult result = new CustomerInsertResult();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_insert_customer", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@customer_code", customerCode);
				cmd.Parameters.AddWithValue("@customer_name", customerName);

				await conn.OpenAsync();

				using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						result.Success = Convert.ToInt32(reader["Success"]) == 1;
						result.Message = reader["Message"]?.ToString() ?? string.Empty;

						result.CustomerId = reader["CustomerId"] == DBNull.Value
							? null
							: Convert.ToInt32(reader["CustomerId"]);
					}
				}
			}

			return result;
		}


		public async Task<OperationResult> InsertProductAsync(string productName,string? gml,string? modelDescription,int? customerId)
		{
			using SqlConnection conn = new SqlConnection(_connectionString);
			using SqlCommand cmd = new SqlCommand("trrs_sp_insert_product", conn);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@product_name", productName);
			cmd.Parameters.AddWithValue("@gml", (object?)gml ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@model_description", (object?)modelDescription ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@customer_id", (object?)customerId ?? DBNull.Value);

			await conn.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			if (await reader.ReadAsync())
			{
				return new OperationResult
				{
					Success = Convert.ToInt32(reader["Success"]) == 1,
					Message = reader["Message"]?.ToString() ?? string.Empty
				};
			}

			return new OperationResult
			{
				Success = false,
				Message = "Unknown error."
			};
		}


		public async Task<bool> UpdateApprovalNotesAsync(int trrsId,int approvalId,string notes)
		{
			using var con = new SqlConnection(_connectionString);
			using var cmd = new SqlCommand("trrs_sp_update_approval_notes", con);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@trrs_id", trrsId);
			cmd.Parameters.AddWithValue("@approval_id", approvalId);
			cmd.Parameters.AddWithValue("@notes", notes ?? string.Empty);

			await con.OpenAsync();

			var result = await cmd.ExecuteScalarAsync();

			int affectedRows = result != null
				? Convert.ToInt32(result)
				: 0;

			return affectedRows > 0;
		}


	}

	
}
