using Data.Models;
using Service.Models;

namespace Service.Services
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllGenres();

        Task<Genre?> GetGenreById(int id);

        Task<bool> AddGenre(GenreRequestModel book);

        Task<bool> UpdateGenre(GenreRequestModel book);

        Task<bool> DeleteGenre(Genre genre);

        Task<List<Genre>> SearchGenres(string query);
    }
}
