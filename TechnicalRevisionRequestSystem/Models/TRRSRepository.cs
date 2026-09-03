using System.Data;

namespace TechnicalRevisionRequestSystem.Models
{
	public interface TRRSRepositoryInsert
	{
		Task<int> InsertRequestAsync(InsertTrrsRequestModel model);
		Task<int> InsertRootCauseAttachmentAsync(int trrsId, string fileName, byte[] fileData);
		Task<int> InsertActionPlanAttachmentAsync(int trrsId, string fileName, byte[] fileData);
		Task<int> InsertApprovalAttachmentAsync(int trrsId, int approvingPart, string fileName, byte[] fileData);
		Task<bool> SaveProductStatus(ProductStatusModel model);
		Task<int> InsertActualActivityInfo(ActualActivityInfoModel model);
		Task<int> InsertProductionSchedule(ProductionScheduleModel model);
		Task<bool> SaveEvaluationResult(EvaluationResultModel model);
		Task<bool> SaveQualityVerification(QualityVerificationModel model);
		Task<int> InsertDeviationDetail(DeviationDetailModel model);
		Task<int> InsertMomAttachmentAsync(int trrsId, string fileName, byte[] fileData, string? remarks);
		Task<CustomerInsertResult> InsertCustomerAsync(string customerCode, string customerName);
		Task<OperationResult> InsertProductAsync(string productName, string? gml, string? modelDescription, int? customerId);
		Task<bool> UpdateApprovalNotesAsync(int trrsId, int approvalId, string notes);


	}

	public interface TRRSRepositorySelect
	{
	
		List<CustomerLookupModel> GetCustomerLookup();
		List<ProductLookupModel> GetProductLookup();
		List<TrrsControlModel> GetTrrsControl();
		Task<List<TrrsRequestListModel>> GetRequestListAsync();
		Task<List<ApprovalModel>> GetRequestApprovalsAsync(int trrsId);
		Task<TrrsRequestDetailsModel?> GetRequestDetailsAsync(int trrsId);
		List<ApprovalHistoryModel> GetApprovalHistory(int trrsId, int approverPart);
		List<RootCauseAttachmentModel> GetRootCauseAttachments(int trrsId);
		List<ActionPlanAttachmentModel> GetActionPlanAttachments(int trrsId);
		RootCauseAttachmentModel GetRootCauseAttachment(int attachmentId);
		ActionPlanAttachmentModel GetActionPlanAttachment(int attachmentId);
		List<ApproverAttachmentModel> GetApproverAttachments(int trrsId, int approvingPart);
		ApproverAttachmentModel GetApproverAttachmentById(int attachmentId);
		ProductStatusModel GetProductStatus(int trrsId);
		Task<ActualActivityInfoModel?> GetActualActivityInfo(int trrsId);
		Task<ProductionScheduleModel?> GetProductionSchedule(int trrsId);
		Task<EvaluationResultModel> GetEvaluationResultAsync(int trrsId);
		Task<QualityVerificationModel?> GetQualityVerification(int trrsId);
		Task<List<DeviationDetailModel>> GetDeviationDetails(int trrsId, int? deviationId = null);
		Task<List<TrrsRequestListModel>> GetMyApprovalRequestListAsync(int deptId, int sectionId);
		List<MomAttachmentModel> GetMomAttachments(int trrsId);
		MomAttachmentModel GetMomAttachment(int attachmentId);
		Task<List<TrrsExportModel>> GetRequestListReportAsync();
		Task<DataTable> GetApprovalsHorizontalAsync(int? trrsId = null);
		Task<List<ProductLookupModel>> GetProductsByCustomerAsync(int customerId);
		Task<AIRequestContext?> GetAIRequestContextAsync(int trrsId);
		Task<List<AISearchRequestContext>> GetAISearchRequestContextAsync();


		//Dashboard


		Task<DataTable> GetDashboardSummaryAsync(int? year, int? month);
		Task<DataTable> GetMonthlyRequestsAsync(int year);
		Task<DataTable> GetRequestsByTypeAsync(int? year, int? month);
		Task<DataTable> GetRequestsByCustomerAsync(int? year, int? month);
		Task<DataTable> GetRequestsByProductAsync(int? year, int? month);
		Task<DataTable> GetRequestsByMoldAsync(int? year, int? month);
		Task<DataTable> GetParentChildSummaryAsync(int? year, int? month);
		Task<DataTable> GetRevisionDistributionAsync(int? year, int? month);
		Task<DataTable> GetTopRequestedProductsAsync(int? year, int? month);
		Task<DataTable> GetDetailedBreakdownAsync(int? year, int? month);
		Task<DataTable> GetMonthlyTrendAsync(int? year, int? month);
		Task<DataTable> GetTRRFDistributionByLifeUtilizationAsync(int? year, int? month);
		Task<DataTable> GetTRRFDetailsByStageIdAsync(int stageId, int? year, int? month);


	}

	public interface TRRSRepositoryUpdate
	{

		ApproveRequestResult ApproveRequest(ApproveRequestModel model);
		bool UpdatePartModifiedInfo(ActualActivityInfoModel model);
		bool UpdateMoldResultInfo(ActualActivityInfoModel model);
		bool UpdateRootCauseActionPlan(TrrsRequestDetailsModel model);
		Task<CustomerInsertResult> DeleteCustomerAsync(int customerId);
		Task<OperationResult> UpdateCustomerAsync(int customerId, string customerCode, string customerName);
		Task<OperationResult> UpdateProductAsync(int productId, string productName, string gml);
		Task<OperationResult> DeleteProductAsync(int productId);
		Task<OperationResult> RestoreProductAsync(int productId);
		Task<OperationResult> UpdateProductBindAsync(int productId, string productName, string? gml, string? modelDescription, int? customerId);



	}
}
