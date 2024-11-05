using Business;
using Data.Models;
using Service.Models;

namespace Service.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ILogger<AuthorService> _logger;
        private readonly AuthorRepository _authorRepository;

        public AuthorService(ILogger<AuthorService> logger)
        {
            _logger = logger;
            _authorRepository = new AuthorRepository();
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            return await Task.Run(() => _authorRepository.GetAll());
        }

        public async Task<Author?> GetAuthorById(int id)
        {
            return await Task.Run(() => _authorRepository.FindById(id));
        }

        public async Task<bool> AddAuthor(AuthorRequestModel author)
        {
            Author entity = new()
            {
                Firstname = author.Firstname,
                Lastname = author.Lastname,
                Birthdate = author.Birthdate,
                Pseudonym = author.Pseudonym,
            };

            return await Task.Run(() => _authorRepository.Create(entity));
        }

        public async Task<bool> UpdateAuthor(AuthorRequestModel author)
        {
            if (!author.Id.HasValue)
            {
                throw new Exception("El id del autor es necesario");
            }

            Author entity = _authorRepository.FindById(author.Id.Value) 
                ?? throw new Exception("El autor no existe o fue eliminado");

            entity.Id = author.Id.Value;
            entity.Firstname = author.Firstname;
            entity.Lastname = author.Lastname;
            entity.Birthdate = author.Birthdate;
            entity.Pseudonym = author.Pseudonym;

            return await Task.Run(() => _authorRepository.Update(entity));
        }

        public async Task<bool> DeleteAuthor(Author author)
        {
            return await Task.Run(() => _authorRepository.Delete(author));
        }

        public async Task<List<Author>> SearchAuthors(string query)
        {
            return await Task.Run(() => _authorRepository.FindAll(author =>
            {
                List<string> fields = [author.Firstname, author.Lastname, author.Pseudonym];
                fields.RemoveAll(x => x == null);

                if (fields.Count == 0)
                {
                    return false;
                }

                return fields.Any(x => x.Trim().Contains(query.Trim()));
            }));
        }
    }
}
