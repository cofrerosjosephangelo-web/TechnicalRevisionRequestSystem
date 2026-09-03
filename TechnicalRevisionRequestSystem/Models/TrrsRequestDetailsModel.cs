namespace TechnicalRevisionRequestSystem.Models
{
	public class TrrsRequestDetailsModel
	{
		public int TrrsId { get; set; }
		public string TrrfNo { get; set; }
		public DateTime? DatePrepared { get; set; }
		public string TrrsType { get; set; }

		public int CustomerId { get; set; }
		public string CustomerName { get; set; }

		public int ProductId { get; set; }
		public string ProductName { get; set; }

		public string MoldNo { get; set; }
		public string PartModelNo { get; set; }

		public string EncounteredProblem { get; set; }
		public string MachineName { get; set; }
		public int? MoldToolLife { get; set; }

		// Root Cause
		public bool RcWearTear { get; set; }
		public bool RcMachineError { get; set; }
		public bool RcDesignError { get; set; }
		public bool RcFabricationError { get; set; }
		public bool RcEffectOfPrevImprovement { get; set; }
		public bool RcCustomerRequirement { get; set; }
		public string RcDetails { get; set; }

		// Action Plan
		public bool ApRepair { get; set; }
		public bool ApAdjustment { get; set; }
		public bool ApRevision { get; set; }
		public bool ApReplacement { get; set; }
		public bool ApTrialTesting { get; set; }
		public string ApDetails { get; set; }

		// Containment
		public bool HasSpare { get; set; }
		public int? Quantity { get; set; }
		public bool? rcattachment { get; set; }
		public bool? apattachment { get; set; }
		public bool? momattachment { get; set; }
		public string? momremarks { get; set; }

		public string ControlDisplay { get; set; }
	}

	public class RootCauseAttachmentModel
	{
		public int attachment_id { get; set; }
		public int trrs_id { get; set; }
		public string file_name { get; set; }
		public byte[] file_data { get; set; }
	}

	public class ActionPlanAttachmentModel
	{
		public int attachment_id { get; set; }
		public int trrs_id { get; set; }
		public string file_name { get; set; }
		public byte[] file_data { get; set; }
	}

	public class MomAttachmentModel
	{
		public int attachment_id { get; set; }
		public int trrs_id { get; set; }
		public string file_name { get; set; }
		public byte[] file_data { get; set; }
	}


	public class ApprovalHistoryModel
	{
		public int approval_id { get; set; }

		public int trrs_id { get; set; }

		public string approved_by { get; set; }

		public DateTime? approved_date { get; set; }

		public string approver_remarks { get; set; }

		public int approver_part { get; set; }

		public string approver_role { get; set; }

		public bool done_approve { get; set; }

		public int? for_approve { get; set; }

		public int? dept_id { get; set; }

		public int? parent_dept_id { get; set; }

		public int? section_id { get; set; }

		public bool? has_attachment { get; set; }
	}

	public class ApproverAttachmentModel
	{
		public int attachment_id { get; set; }
		public int trrs_id { get; set; }
		public int approving_part { get; set; }
		public string file_name { get; set; }
		public byte[] file_data { get; set; }
	}


}
