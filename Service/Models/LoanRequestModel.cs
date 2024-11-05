namespace Service.Models
{
	public class LoanRequestModel
	{
		public int? Id { get; set; }
		public int BookId { get; set; }
		public string IdentificationNumber { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Status { get; set; } = "PENDING";
		public DateTime? LoanedOn { get; set; }
	}
}
