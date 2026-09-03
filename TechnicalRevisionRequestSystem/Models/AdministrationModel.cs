namespace TechnicalRevisionRequestSystem.Models
{
	public class AdministrationModel
	{
	}

	public class CustomerInsertResult
	{
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
		public int? CustomerId { get; set; }
	}

	public class OperationResult
	{
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
	}

	public class ProductAdministrationViewModel
	{
		public List<ProductLookupModel> Products { get; set; } = new();
		public List<CustomerLookupModel> Customers { get; set; } = new();
	}
}
