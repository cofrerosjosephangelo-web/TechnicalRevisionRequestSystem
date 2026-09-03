using System.Runtime.InteropServices;

namespace TechnicalRevisionRequestSystem.Models
{
	public class ApproveRequestModel
	{
		public int trrs_id { get; set; }
		public string approved_by { get; set; }
		public DateTime approved_date { get; set; }
		public string approver_remarks { get; set; }
		public int approving_part { get; set; }

		public IFormFile[]? ApproverAttachments { get; set; }



		//Product Status

		
		public bool is_rush { get; set; }
		public bool is_next_production { get; set; }
		public DateTime? status_as_of { get; set; }
		public DateTime? required_date { get; set; }
		public string? status_details { get; set; }
		public string current_role { get; set; }



		//Actual activity info
		public string designchangedetails { get; set; }
		public DateTime? dateaccomplishedchange { get; set; }
		public DateTime? datereceived { get; set; }

		public string partmodifieddetails { get; set; }
		public DateTime? dateaccomplishedmodified { get; set; }

		public string moldresult { get; set; }
		public DateTime? dateaccomplishedresult { get; set; }



		//production schedule

		public DateTime? proddatereceived { get; set; }
		public DateTime? prodtesting { get; set; }


		//evaluation result

		public int evaluation_id { get; set; }

		public string? productresult { get; set; }

		public bool isapproved { get; set; }



		//job order

		public string? job_order { get; set; }


		public string? trrs_type { get; set; }



		//with deviation details

		public int deviation_detail_id { get; set; }

		public int? deviation_id { get; set; }

		public string? deviation_details { get; set; }

		public DateTime? deviation_date { get; set; }

		public int? withdeviation { get; set; }


		public bool rcWearTear { get; set; }
		public bool rcMachineError { get; set; }
		public bool rcDesignError { get; set; }
		public bool rcFabricationError { get; set; }
		public bool rcEffectOfPrevImprovement { get; set; }
		public bool rcCustomerRequirement { get; set; }
		public string rcDetails { get; set; }

		// Action Plan
		public bool apRepair { get; set; }
		public bool apAdjustment { get; set; }
		public bool apRevision { get; set; }
		public bool apReplacement { get; set; }
		public bool apTrialTesting { get; set; }
		public string apDetails { get; set; }

		public IFormFile[]? RootCauseAttachments { get; set; }
		public IFormFile[]? ActionPlanAttachments { get; set; }

	}

	public class ApproveRequestResult
	{
		public bool Success { get; set; }
		public int? NewTRRSId { get; set; }
	}

	public class ProductStatusModel
	{
		public int product_status_id { get; set; }
		public int trrs_id { get; set; }
		public bool is_rush { get; set; }
		public bool is_next_production { get; set; }
		public DateTime? status_as_of { get; set; }
		public DateTime? required_date { get; set; }
		public string? status_details { get; set; }
	}



	public class ActualActivityInfoModel
	{
		public int trrs_id { get; set; }

		public string designchangedetails { get; set; }
		public DateTime? dateaccomplishedchange { get; set; }
		public DateTime? datereceived { get; set; }

		public string partmodifieddetails { get; set; }
		public DateTime? dateaccomplishedmodified { get; set; }

		public string moldresult { get; set; }
		public DateTime? dateaccomplishedresult { get; set; }
	}


	public class ProductionScheduleModel
	{
		public int trrs_id { get; set; }
		public int sched_id { get; set; }
		public DateTime? proddatereceived { get; set; }
		public DateTime? prodtesting { get; set; }
	}

	public class EvaluationResultModel
	{
		public int trrs_id { get; set; }

		public int evaluation_id { get; set; }	

		public string? productresult { get; set; }

		public bool isapproved { get; set; }
	}


	public class QualityVerificationModel
	{
		public int verification_id { get; set; }

		public int trrs_id { get; set; }

		public string? job_order { get; set; }
	}


	public class DeviationDetailModel
	{
		public int deviation_detail_id { get; set; }

		public int? deviation_id { get; set; }

		public string? deviation_details { get; set; }

		public DateTime? deviation_date { get; set; }


	
		public int trrs_id { get; set; }
		public int deviation_version { get; set; }
	}



}
