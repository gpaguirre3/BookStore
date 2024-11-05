using DesktopClient.Models;

namespace DesktopClient.Services
{
    public class BookService : IBookService
    {
        public Task AddBookAsync(BookModel book)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBookAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<BookModel> GetBookAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookModel>> GetBooksAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookAsync(BookModel book)
        {
            throw new NotImplementedException();
        }
    }
}
