using PresentationWeb.ApiService;

namespace PresentationWeb.Models
{
    public class AuthorsIndexViewModel
    {
        public List<Author> Authors { get; set; } = [];
        public List<string> Errors { get; set; } = [];
    }
}
