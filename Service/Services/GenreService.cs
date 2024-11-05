using Business;
using Data.Models;
using Service.Models;

namespace Service.Services
{
    public class GenreService : IGenreService
    {
        private readonly ILogger<GenreService> _logger;
        private readonly GenreRepository _genreRepository;

        public GenreService(ILogger<GenreService> logger)
        {
            _logger = logger;
            _genreRepository = new GenreRepository();
        }

        public async Task<List<Genre>> GetAllGenres()
        {
            return await Task.Run(() => _genreRepository.GetAll());
        }

        public async Task<Genre?> GetGenreById(int id)
        {
            return await Task.Run(() => _genreRepository.FindById(id));
        }

        public async Task<bool> AddGenre(GenreRequestModel genre)
        {
            Genre entity = new()
            {
                Name = genre.Name,
            };

            return await Task.Run(() => _genreRepository.Create(entity));
        }

        public async Task<bool> UpdateGenre(GenreRequestModel genre)
        {
            if (!genre.Id.HasValue)
            {
                throw new Exception("El id del autor es necesario");
            }

            Genre entity = _genreRepository.FindById(genre.Id.Value)
                ?? throw new Exception("El autor no existe o fue eliminado");

            entity.Id = genre.Id.Value;
            entity.Name = genre.Name;

            return await Task.Run(() => _genreRepository.Update(entity));
        }

        public async Task<bool> DeleteGenre(Genre genre)
        {
            return await Task.Run(() => _genreRepository.Delete(genre));
        }

        public async Task<List<Genre>> SearchGenres(string query)
        {
            return await Task.Run(() =>
            {
                return _genreRepository.FindAll(g => g.Name != null 
                    && g.Name.Trim().Contains(query.Trim(), StringComparison.CurrentCultureIgnoreCase));
            });
        }
    }
}
