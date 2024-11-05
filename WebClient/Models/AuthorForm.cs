namespace PresentationWeb.Models
{
	public class AuthorFormModel
	{
		public int? Id { get; set; }
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string Pseudonym { get; set; } = string.Empty;
	}
}
