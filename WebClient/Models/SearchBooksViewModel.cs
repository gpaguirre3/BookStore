using PresentationWeb.ApiService;

namespace PresentationWeb.Models
{
    public class SearchBooksViewModel
    {
        public List<Book> Books { get; set; } = [];
        public string Query { get; set; } = string.Empty;
    }
}
