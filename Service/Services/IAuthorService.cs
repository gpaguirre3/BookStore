using Data.Models;
using Service.Models;

namespace Service.Services
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAuthors();

        Task<Author?> GetAuthorById(int id);

        Task<bool> AddAuthor(AuthorRequestModel book);

        Task<bool> UpdateAuthor(AuthorRequestModel book);

        Task<bool> DeleteAuthor(Author author);

        Task<List<Author>> SearchAuthors(string query);
    }
}
