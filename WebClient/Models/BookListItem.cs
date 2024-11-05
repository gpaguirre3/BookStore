using PresentationWeb.ApiService;

namespace PresentationWeb.Models
{
    public class BookListItem : Book
    {
        public bool IsLend { get; set; } = false;
    }
}
