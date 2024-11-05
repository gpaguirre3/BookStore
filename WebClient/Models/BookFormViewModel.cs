using PresentationWeb.ApiService;

namespace PresentationWeb.Models
{
    public class BookFormViewModel
    {
        public Book? BookContext { get; set; }
        public List<Author> Authors { get; set; } = [];
        public List<Genre> Genres { get; set; } = [];
        public List<string> Errors { get; set; } = [];
    }
}
