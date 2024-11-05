using PresentationWeb.ApiService;

namespace PresentationWeb.Models
{
    public class AuthorFormViewModel
    {
        public Author? Author { get; set; } = null;
        public List<string> Errors { get; set; } = [];
    }
}
