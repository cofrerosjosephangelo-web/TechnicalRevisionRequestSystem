namespace TechnicalRevisionRequestSystem.Models
{
	public class InsertTrrsRequestModel
	{
		public string Type { get; set; } = string.Empty; // MP or PD

		public string CreateUser { get; set; } = string.Empty;
		public int DeptId { get; set; }
		public int SectionId { get; set; }
		public int ParentId { get; set; }

		public int CustomerId { get; set; } 
		public int ProductId { get; set; } 
		public int MoldNo { get; set; } 
		public string PartModelNo { get; set; } = string.Empty;
		public DateTime DatePrepared { get; set; }

		public string CreateUserId { get; set; } = string.Empty;
		public string CreateUserName { get; set; } = string.Empty;

		public string EncounteredProblem { get; set; } = string.Empty;
		public string MachineName { get; set; } = string.Empty;
		public int MoldToolLife { get; set; } 

		public bool RcWearTear { get; set; }
		public bool RcMachineError { get; set; }
		public bool RcDesignError { get; set; }
		public bool RcFabricationError { get; set; }
		public bool RcEffectOfPrevImprovement { get; set; }
		public bool RcCustomerRequirement { get; set; }
		public string RcDetails { get; set; } = string.Empty;

		public bool ApRepair { get; set; }
		public bool ApAdjustment { get; set; }
		public bool ApRevision { get; set; }
		public bool ApReplacement { get; set; }
		public bool ApTrialTesting { get; set; }
		public string ApDetails { get; set; } = string.Empty;

		public bool HasSpare { get; set; }
		public int Quantity { get; set; }
		public string productname { get; set; } = string.Empty;


		public IFormFile[]? RootCauseAttachments { get; set; }
		public IFormFile[]? ActionPlanAttachments { get; set; }
		public IFormFile[]? MeetingMinutesAttachments { get; set; }

		public string? MeetingMinutesRemarksTxt { get; set; }

	}

	public class CustomerLookupModel
	{
		public int customerId { get; set; }
		public string customerCode { get; set; }

		public string customersName { get; set; }
		public string customerName { get; set; }
	}

	public class ProductLookupModel
	{
		public int productId { get; set; }
		public string productName { get; set; }
		public int gml { get; set; }
		public int? customerid { get; set; }
		public string? modelpartno { get; set; }
		public int? isdeleted { get; set; }
	}

	public class TrrsControlModel
	{
		public int id { get; set; }
		public string controlName { get; set; }
		public string controlCode { get; set; }
		public int controlNumber { get; set; }
		public string controlDisplay { get; set; }
	}




}
