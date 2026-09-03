using Microsoft.Data.SqlClient;
using System.Data;

namespace TechnicalRevisionRequestSystem.Models
{
	public class USQLDB : TRRSRepositoryUpdate
	{

		private readonly string _connectionString;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public USQLDB(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_connectionString = configuration.GetConnectionString("TRRSConnection");
			_httpContextAccessor = httpContextAccessor;
		}

		public ApproveRequestResult ApproveRequest(ApproveRequestModel model)
		{
			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_approve_request", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@trrs_id", SqlDbType.Int).Value = model.trrs_id;
				cmd.Parameters.Add("@approved_by", SqlDbType.NVarChar, 100).Value = model.approved_by;
				cmd.Parameters.Add("@approved_date", SqlDbType.DateTime).Value = model.approved_date;
				cmd.Parameters.Add("@approver_remarks", SqlDbType.NVarChar).Value =
					string.IsNullOrWhiteSpace(model.approver_remarks)
						? DBNull.Value
						: model.approver_remarks;

				cmd.Parameters.Add("@withdeviation", SqlDbType.Int).Value = model.withdeviation;

				// Root Cause
				cmd.Parameters.AddWithValue("@RcWearTear", model.rcWearTear);
				cmd.Parameters.AddWithValue("@RcMachineError", model.rcMachineError);
				cmd.Parameters.AddWithValue("@RcDesignError", model.rcDesignError);
				cmd.Parameters.AddWithValue("@RcFabricationError", model.rcFabricationError);
				cmd.Parameters.AddWithValue("@RcEffectOfPrevImprovement", model.rcEffectOfPrevImprovement);
				cmd.Parameters.AddWithValue("@RcCustomerRequirement", model.rcCustomerRequirement);
				cmd.Parameters.AddWithValue("@RcDetails", (object?)model.rcDetails ?? DBNull.Value);

				// Action Plan
				cmd.Parameters.AddWithValue("@ApRepair", model.apRepair);
				cmd.Parameters.AddWithValue("@ApAdjustment", model.apAdjustment);
				cmd.Parameters.AddWithValue("@ApRevision", model.apRevision);
				cmd.Parameters.AddWithValue("@ApReplacement", model.apReplacement);
				cmd.Parameters.AddWithValue("@ApTrialTesting", model.apTrialTesting);
				cmd.Parameters.AddWithValue("@ApDetails", (object?)model.apDetails ?? DBNull.Value);

				con.Open();

				var result = new ApproveRequestResult();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						result.Success = Convert.ToBoolean(reader["Success"]);

						result.NewTRRSId = reader["NewTRRSId"] == DBNull.Value
							? null
							: Convert.ToInt32(reader["NewTRRSId"]);
					}
				}

				return result;
			}
		}


		public bool UpdatePartModifiedInfo(ActualActivityInfoModel model)
		{
			int rowsAffected = 0;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_update_part_modified_info", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);

					cmd.Parameters.AddWithValue("@part_modified_details",
						(object?)model.partmodifieddetails ?? DBNull.Value);

					cmd.Parameters.AddWithValue("@date_accomplished_modified",
						(object?)model.dateaccomplishedmodified ?? DBNull.Value);

					conn.Open();

					rowsAffected = Convert.ToInt32(cmd.ExecuteScalar());
				}
			}

			return rowsAffected > 0;
		}

		public bool UpdateMoldResultInfo(ActualActivityInfoModel model)
		{
			int rowsAffected = 0;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_update_mold_result_info", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@trrs_id", model.trrs_id);

					cmd.Parameters.AddWithValue("@mold_result",
						(object?)model.moldresult ?? DBNull.Value);

					cmd.Parameters.AddWithValue("@date_accomplished_result",
						(object?)model.dateaccomplishedresult ?? DBNull.Value);

					conn.Open();

					rowsAffected = Convert.ToInt32(cmd.ExecuteScalar());
				}
			}

			return rowsAffected > 0;
		}




		public bool UpdateRootCauseActionPlan(TrrsRequestDetailsModel model)
		{
			int rowsAffected = 0;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand("trrs_sp_update_root_cause_action_plan", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@TrrsId", model.TrrsId);

					// Root Cause
					cmd.Parameters.AddWithValue("@RcWearTear", model.RcWearTear);
					cmd.Parameters.AddWithValue("@RcMachineError", model.RcMachineError);
					cmd.Parameters.AddWithValue("@RcDesignError", model.RcDesignError);
					cmd.Parameters.AddWithValue("@RcFabricationError", model.RcFabricationError);
					cmd.Parameters.AddWithValue("@RcEffectOfPrevImprovement", model.RcEffectOfPrevImprovement);
					cmd.Parameters.AddWithValue("@RcCustomerRequirement", model.RcCustomerRequirement);
					cmd.Parameters.AddWithValue("@RcDetails",
						(object?)model.RcDetails ?? DBNull.Value);

					// Action Plan
					cmd.Parameters.AddWithValue("@ApRepair", model.ApRepair);
					cmd.Parameters.AddWithValue("@ApAdjustment", model.ApAdjustment);
					cmd.Parameters.AddWithValue("@ApRevision", model.ApRevision);
					cmd.Parameters.AddWithValue("@ApReplacement", model.ApReplacement);
					cmd.Parameters.AddWithValue("@ApTrialTesting", model.ApTrialTesting);
					cmd.Parameters.AddWithValue("@ApDetails",
						(object?)model.ApDetails ?? DBNull.Value);

					conn.Open();

					rowsAffected = Convert.ToInt32(cmd.ExecuteScalar());
				}
			}

			return rowsAffected > 0;
		}


		public async Task<CustomerInsertResult> DeleteCustomerAsync(int customerId)
		{
			CustomerInsertResult result = new CustomerInsertResult();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_delete_customer", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@customer_id", customerId);

				await conn.OpenAsync();

				using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						result.Success = Convert.ToInt32(reader["Success"]) == 1;
						result.Message = reader["Message"]?.ToString() ?? string.Empty;
					}
				}
			}

			return result;
		}

		public async Task<OperationResult> UpdateCustomerAsync(int customerId,string customerCode,string customerName)
		{
			OperationResult result = new OperationResult();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("trrs_sp_update_customer", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@customer_id", customerId);
				cmd.Parameters.AddWithValue("@customer_code", customerCode);
				cmd.Parameters.AddWithValue("@customer_name", customerName);

				await conn.OpenAsync();

				using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						result.Success = Convert.ToInt32(reader["Success"]) == 1;
						result.Message = reader["Message"]?.ToString() ?? string.Empty;
					}
				}
			}

			return result;
		}

		public async Task<OperationResult> UpdateProductAsync(int productId,string productName,string gml)
		{
			using SqlConnection conn = new SqlConnection(_connectionString);
			using SqlCommand cmd = new SqlCommand("trrs_sp_update_product", conn);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@prod_id", productId);
			cmd.Parameters.AddWithValue("@product_name", productName);
			cmd.Parameters.AddWithValue("@gml", (object?)gml ?? DBNull.Value);

			await conn.OpenAsync();

			using SqlDataReader reader = await cmd.ExecuteReaderAsync();

			if (await reader.ReadAsync())
			{
				return new OperationResult
				{
					Success = Convert.ToInt32(reader["Success"]) == 1,
					Message = reader["Message"].ToString() ?? string.Empty
				};
			}

			return new OperationResult
			{
				Success = false,
				Message = "Unknown error."
			};
		}

		public async Task<OperationResult> DeleteProductAsync(int productId)
		{
			using SqlConnection conn = new SqlConnection(_connectionString);
			using SqlCommand cmd = new SqlCommand("trrs_sp_delete_product", conn);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@prod_id", productId);

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


		public async Task<OperationResult> RestoreProductAsync(int productId)
		{
			using SqlConnection conn = new SqlConnection(_connectionString);
			using SqlCommand cmd = new SqlCommand("trrs_sp_restore_product", conn);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@prod_id", productId);

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


		public async Task<OperationResult> UpdateProductBindAsync(int productId,string productName,string? gml,string? modelDescription,int? customerId)
		{
			using SqlConnection conn = new SqlConnection(_connectionString);
			using SqlCommand cmd = new SqlCommand("trrs_sp_update_product", conn);

			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@product_id", productId);
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


	}


}
