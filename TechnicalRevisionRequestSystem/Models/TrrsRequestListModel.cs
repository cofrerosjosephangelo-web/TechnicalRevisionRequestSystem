namespace TechnicalRevisionRequestSystem.Models
{
	public class TrrsRequestListModel
	{
		public string controlno { get; set; }
		public string trrfno { get; set; }
		public string customername { get; set; }
		public string productname { get; set; }
		public int moldno { get; set; }
		public string partmodelno { get; set; }
		public string machinename { get; set; }
		public string problem { get; set; }
		public string submittedby { get; set; }
		public DateTime dateprepared { get; set; }
		public int id { get; set; }
		public string trrstype { get; set; }
		public string currentstatus { get; set; }
		public int currentpart { get; set; }
		public int? revisionno { get; set; }
		public int? deviationid { get; set; }
		public string moldreference { get; set; }
		public string? notes { get; set; }
		public int approvalId { get; set; }
	}

	public class ApprovalModel
	{
		public int part { get; set; }
		public string role { get; set; }
		public string approvedby { get; set; }
		public DateTime? approveddate { get; set; }
		public string remarks { get; set; }
		public int done { get; set; }
		public int forapprove { get; set; }
		public int deptid { get; set; }
		public int sectionid { get; set; }
		public int id { get; set; }
		public string trrstype { get; set; }
		public string? deviationid { get; set; }
		public string? notes { get; set; }
		public int approvalId { get; set; }
	}

	public class TrrsExportModel
	{
		public string ControlNo { get; set; }
		public string TRRFNo { get; set; }
		public int RevisionNo { get; set; }
		public string Type { get; set; }
		public string Customer { get; set; }
		public string Product { get; set; }
		public int MoldNo { get; set; }
		public string PartModelNo { get; set; }
		public string Problem { get; set; }
		public string MachineName { get; set; }
		public string SubmittedBy { get; set; }
		public string CurrentStatus { get; set; }
		public string CurrentPart { get; set; }
		public DateTime DatePrepared { get; set; }
	}
}
