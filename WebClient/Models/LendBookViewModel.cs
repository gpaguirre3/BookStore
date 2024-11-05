using PresentationWeb.ApiService;

namespace PresentationWeb.Models
{
    public class LendBookViewModel
    {
        public Book TargetBook { get; set; } = new Book() { Id = 0 };
        public List<string> Errors { get; set; } = [];

        public string? Email { get; set; }
        public string? IdentificationNumber { get; set; }
    }
}
