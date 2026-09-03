using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Net.Mail;

namespace TechnicalRevisionRequestSystem.Models
{

	public class AskKnowledgeModel
	{
		public string Question { get; set; }
	}

	public class AISearchRequestContext
	{
		// ------------------------------------------------------------
		// TRRF INFORMATION
		// ------------------------------------------------------------

		public int TrrfId { get; set; }

		public string? TrrfCode { get; set; }

		public string? RequestType { get; set; }

		public int RequestVersion { get; set; }

		public DateTime? DatePrepared { get; set; }

		public string? TicketStatus { get; set; }

		public int? CurrentPart { get; set; }

		public int? DeviationId { get; set; }

		public bool? HasRevisionOrDeviation { get; set; }

		public int? AttemptNumber { get; set; }


		// ------------------------------------------------------------
		// REQUESTER INFORMATION
		// ------------------------------------------------------------

		public string? PreparedBy { get; set; }


		// ------------------------------------------------------------
		// CUSTOMER INFORMATION
		// ------------------------------------------------------------

		public string? CustomerCode { get; set; }

		public string? CustomerName { get; set; }


		// ------------------------------------------------------------
		// PRODUCT / MOLD INFORMATION
		// ------------------------------------------------------------

		public string? ProductName { get; set; }

		public int? MoldNumber { get; set; }

		public string? PartModelNumber { get; set; }



		// ------------------------------------------------------------
		// PROBLEM INFORMATION
		// ------------------------------------------------------------

		public string? ProblemDescription { get; set; }

		public string? MachineName { get; set; }

		public int? CurrentMoldToolLife { get; set; }



		// ------------------------------------------------------------
		// ROOT CAUSE INFORMATION
		// ------------------------------------------------------------

		public bool? RootCauseWearTear { get; set; }

		public bool? RootCauseMachineError { get; set; }

		public bool? RootCauseDesignError { get; set; }

		public bool? RootCauseFabricationError { get; set; }

		public bool? RootCausePreviousImprovement { get; set; }

		public bool? RootCauseCustomerRequirement { get; set; }

		public string? RootCauseDetails { get; set; }



		// ------------------------------------------------------------
		// ACTION PLAN INFORMATION
		// ------------------------------------------------------------

		public bool? ActionRepair { get; set; }

		public bool? ActionAdjustment { get; set; }

		public bool? ActionRevision { get; set; }

		public bool? ActionReplacement { get; set; }

		public bool? ActionTrialTesting { get; set; }

		public string? ActionPlanDetails { get; set; }



		// ------------------------------------------------------------
		// PRODUCT STATUS
		// ------------------------------------------------------------

		public bool? IsRushRequest { get; set; }

		public bool? IsNextProduction { get; set; }

		public DateTime? StatusDate { get; set; }

		public DateTime? RequiredDate { get; set; }

		public string? ProductStatusRemarks { get; set; }



		// ------------------------------------------------------------
		// EVALUATION
		// ------------------------------------------------------------

		public string? EvaluationResult { get; set; }

		public bool? EvaluationApproved { get; set; }



		// ------------------------------------------------------------
		// PRODUCTION SCHEDULE
		// ------------------------------------------------------------

		public DateTime? ProductionDateReceived { get; set; }

		public DateTime? ProductionTestingDate { get; set; }



		// ------------------------------------------------------------
		// ACTUAL ACTIVITY
		// ------------------------------------------------------------

		public DateTime? ChangeCompletedDate { get; set; }

		public string? DesignChangeDetails { get; set; }

		public string? PartModificationDetails { get; set; }

		public string? MoldResult { get; set; }

		public DateTime? ModificationCompletedDate { get; set; }

		public DateTime? ResultCompletedDate { get; set; }



		// ------------------------------------------------------------
		// QUALITY VERIFICATION
		// ------------------------------------------------------------

		public string? QualityJobOrder { get; set; }



		// ------------------------------------------------------------
		// MOLD LIFE UTILIZATION
		// ------------------------------------------------------------

		public string? MoldLifeStage { get; set; }

		public decimal? MoldLifeUtilizationPercentage { get; set; }

		public int? CurrentShotCount { get; set; }

		public int? GuaranteedMoldLife { get; set; }

		public int? RemainingMoldLife { get; set; }



		// ------------------------------------------------------------
		// DEVIATION
		// ------------------------------------------------------------

		public bool? HasDeviation { get; set; }

		public int? DeviationCount { get; set; }

		public string? DeviationDetails { get; set; }

		public int? DeviationVersion { get; set; }


		// ------------------------------------------------------------
		// AI SEARCH HELPER
		// ------------------------------------------------------------

		public string? AISearchSummary { get; set; }
	}

	public class AskRequestModel
	{
		public int TrrsId { get; set; }
		public string Question { get; set; } = string.Empty;
	}


	public class AIRequestContext
	{
		public AIProductInfo ProductInfo { get; set; }

		public AIProblemDetails ProblemDetails { get; set; }

		public AIProductStatus ProductStatus { get; set; }

		public AIEvaluationResult EvaluationResult { get; set; }

		public AIProductionSchedule ProductionSchedule { get; set; }

		public AIActualActivity ActualActivity { get; set; }

		public AIQualityVerification QualityVerification { get; set; }

		public List<AIApprovalHistory> ApprovalHistory { get; set; } = new();

		public AILifeUtilization LifeUtilization { get; set; }

		public AIDeviation Deviation { get; set; }

		public List<AIDeviationDetail> DeviationDetails { get; set; } = new();

		public List<AIAttachment> Attachments { get; set; } = new();
	}

	public class AIProductInfo
	{
		public int TrrsId { get; set; }

		public string? TrrfNo { get; set; }

		public string? TrrsType { get; set; }

		public int TrrsVersion { get; set; }

		public int? TrrsParentId { get; set; }

		public DateTime DatePrepared { get; set; }

		public string? CreateUserId { get; set; }

		public string? CreateUserName { get; set; }

		public int CustomerId { get; set; }

		public string? CustomerCode { get; set; }

		public string? CustomerName { get; set; }

		public int ProductId { get; set; }

		public string? ProductName { get; set; }

		public int MoldNo { get; set; }

		public string? PartModelNo { get; set; }

		public int? DeviationId { get; set; }

		public bool IsDeleted { get; set; }
	}

	public class AIProblemDetails
	{
		public int ProblemId { get; set; }

		public int TrrsId { get; set; }

		public string? EncounteredProblem { get; set; }

		public string? MachineName { get; set; }

		public int MoldToolLife { get; set; }

		public bool RcWearTear { get; set; }

		public bool RcMachineError { get; set; }

		public bool RcDesignError { get; set; }

		public bool RcFabricationError { get; set; }

		public bool RcEffectOfPrevImprovement { get; set; }

		public bool RcCustomerRequirement { get; set; }

		public string? RcDetails { get; set; }

		public bool ApRepair { get; set; }

		public bool ApAdjustment { get; set; }

		public bool ApRevision { get; set; }

		public bool ApReplacement { get; set; }

		public bool ApTrialTesting { get; set; }

		public string? ApDetails { get; set; }

		public bool HasSpare { get; set; }

		public int? Quantity { get; set; }
	}


	public class AIProductStatus
	{
		public int ProductStatusId { get; set; }

		public int TrrsId { get; set; }

		public bool IsRush { get; set; }

		public bool IsNextProduction { get; set; }

		public DateTime? StatusAsOf { get; set; }

		public DateTime? RequiredDate { get; set; }

		public string? StatusDetails { get; set; }
	}


	public class AIEvaluationResult
	{
		public int EvaluationId { get; set; }

		public int TrrsId { get; set; }

		public string? ProductResult { get; set; }

		public bool? IsApproved { get; set; }
	}


	public class AIProductionSchedule
	{
		public int SchedId { get; set; }

		public int TrrsId { get; set; }

		public DateTime? DateReceived { get; set; }

		public DateTime? DateTesting { get; set; }
	}


	public class AIActualActivity
	{
		public int ActivityId { get; set; }

		public int TrrsId { get; set; }

		public DateTime? DateReceived { get; set; }

		public DateTime? DateAccomplishedChange { get; set; }

		public string? DesignChangeDetails { get; set; }

		public string? PartModifiedDetails { get; set; }

		public string? MoldResult { get; set; }

		public DateTime? DateAccomplishedModified { get; set; }

		public DateTime? DateAccomplishedResult { get; set; }
	}


	public class AIQualityVerification
	{
		public int VerificationId { get; set; }

		public int TrrsId { get; set; }

		public string? JobOrder { get; set; }
	}


	public class AIApprovalHistory
	{
		public int ApprovalId { get; set; }

		public string? ApproverRole { get; set; }

		public int ApproverPart { get; set; }

		public string? ApprovedBy { get; set; }

		public DateTime? ApprovedDate { get; set; }

		public string? ApproverRemarks { get; set; }

		public int DoneApprove { get; set; }

		public int ForApprove { get; set; }

		public int DeptId { get; set; }

		public int ParentDeptId { get; set; }

		public int SectionId { get; set; }
	}


	public class AILifeUtilization
	{
		public int DataId { get; set; }

		public int TrrsId { get; set; }

		public int ConditionStageId { get; set; }

		public decimal UtilizationPercentage { get; set; }

		public int ComputedShots { get; set; }

		public int GuaranteedLife { get; set; }

		public int RemainingShots { get; set; }

		public DateTime CreatedDate { get; set; }

		public string? CreatedBy { get; set; }

		public string? ConditionStage { get; set; }

		public decimal PercentageMin { get; set; }

		public decimal? PercentageMax { get; set; }
	}


	public class AIDeviation
	{
		public int DeviationId { get; set; }

		public int TrrsId { get; set; }

		public bool WithDeviation { get; set; }

		public int DeviationCount { get; set; }
	}


	public class AIDeviationDetail
	{
		public int DeviationDetailId { get; set; }

		public int DeviationId { get; set; }

		public string? DeviationDetails { get; set; }

		public DateTime? DeviationDate { get; set; }

		public int DeviationVersion { get; set; }
	}


	public class AIAttachment
	{
		public string? AttachmentType { get; set; }

		public string? FileName { get; set; }

		public string? Remarks { get; set; }
	}
}
