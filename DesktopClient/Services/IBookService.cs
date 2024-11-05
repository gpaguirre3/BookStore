using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopClient.Models;

namespace DesktopClient.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookModel>> GetBooksAsync();
        Task<BookModel> GetBookAsync(long id);
        Task AddBookAsync(BookModel book);
        Task UpdateBookAsync(BookModel book);
        Task DeleteBookAsync(long id);
    }
}
