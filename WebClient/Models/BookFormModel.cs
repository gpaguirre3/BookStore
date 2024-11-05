namespace PresentationWeb.Models
{
	public class BookFormModel
	{
		public int? Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Isbn { get; set; } = string.Empty;
		public int Year { get; set; }
		public string Publisher { get; set; } = string.Empty;
		public double Price { get; set; }
		public int AuthorId { get; set; }
		public List<Int32> GenreIds { get; set; } = [];
		public IFormFile? Image { get; set; }
		public string? ServerImage { get; set; }
	}
}
