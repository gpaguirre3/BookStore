using System.ComponentModel.DataAnnotations;

namespace PresentationWeb.Models
{
	public class LendFormModel
	{
		public int BookId { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string IdentificationNumber { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		public string Email { get; set; } = string.Empty;
	}
}
