using Data.Models;
using Service.Models;

namespace Service.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooks();

        Task<Book?> GetBookById(int id);

        Task<bool> AddBook(BookRequestModel book);

        Task<bool> UpdateBook(BookRequestModel book);

        Task<bool> DeleteBook(Book book);

        Task<List<Book>> SearchBooks(string query);
    }
}
