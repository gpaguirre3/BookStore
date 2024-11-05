using Business;
using Data.Models;
using Microsoft.IdentityModel.Tokens;
using Service.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Service.Services
{
    public class BookService : IBookService
    {
        // logger
        private readonly ILogger<BookService> _logger;
        private readonly BookRepository _bookRepository;
        private readonly AuthorRepository _authorRepository;
        private readonly GenreRepository _genreRepository;

        public BookService(
            ILogger<BookService> logger)
        {
            _logger = logger;
            _bookRepository = new BookRepository();
            _authorRepository = new AuthorRepository();
            _genreRepository = new GenreRepository();
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await Task.Run(() => _bookRepository.GetAll().OrderByDescending(x => x.Id).ToList());
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await Task.Run(() => _bookRepository.FindById(id));
        }

        public async Task<bool> AddBook(BookRequestModel book)
        {
            ArgumentNullException.ThrowIfNull(book);

            if (book.Genres.IsNullOrEmpty())
            {
                throw new Exception("Se requiere minimo 1 categoría para el libro");
            }

            if (book.Title.IsNullOrEmpty() || book?.Title?.Trim().Length == 0)
            {
                throw new Exception("El titulo no puede estar vacío");
            }

            return await CreateOrUpdateBook(book!);
        }

        public async Task<bool> UpdateBook(BookRequestModel book)
        {
            return await CreateOrUpdateBook(book, true);
        }

        public async Task<bool> DeleteBook(Book book)
        {
            return await Task.Run(() => _bookRepository.Delete(book));
        }

        private async Task<bool> CreateOrUpdateBook(BookRequestModel book, bool update = false)
        {
            int id = book.Id ?? 0;

            if (id == 0 && update)
            {
                throw new Exception("El id del libro es necesario");
            }

            Book newBook;

            if (id != 0)
            {
                newBook = await Task.Run(() => _bookRepository.FindById(id)) ?? throw new Exception("El libro no existe o ha sido eliminado.");
            }
            else
            {
                newBook = new()
                {
                    Id = 0
                };
            }

            newBook.Author = null;
            newBook.Title = book.Title;
            newBook.Price = book.Price;
            newBook.Year = book.Year;
            newBook.Publisher = book.Publisher;
            newBook.ISBN = book.ISBN;

            if (book.Author?.Id.HasValue == true && book.Author.Id.Value > 0)
            {
                var author = _authorRepository.FindById(book.Author.Id.Value);

                if (author != null)
                {
                    newBook.AuthorId = book.Author.Id.Value;
                }
                else if (author == null && !update)
                {
                    throw new Exception("El autor no existe o fue eliminado");
                }
            }
            else
            {
                throw new Exception("El autor es requerido");
            }

            if (book.Genres.Count > 0)
            {
                newBook.Genres.Clear();

                foreach (var item in book.Genres)
                {
                    if (item.Id.HasValue == false)
                    {
                        continue;
                    }

                    var genre = _genreRepository.FindById(item.Id) 
                        ?? throw new Exception($"La categoría con id {item.Id} no existe o fue eliminada");
                    
                    newBook.Genres.Add(new Genre()
                    {
                        Id = genre.Id
                    });
                }
            }
            else
            {
                throw new Exception("Se necesita al menos una categoría para el libro");
            }

            if (book.Image != null && book.Image.Length > 0)
            {
                string filename = UploadBookImage(book);
                newBook.Image = $"http://localhost:5277/Uploads/BookPortrait/{filename}";
            }

            if (update)
            {
                return await Task.Run(() => _bookRepository.Update(newBook));
            }
            else
            {
                return await Task.Run(() => _bookRepository.Create(newBook));
            }
        }

        public async Task<List<Book>> SearchBooks(string query)
        {
            return await Task.Run(() => _bookRepository.FindAll(book =>
            {
                if (book.Title == null)
                {
                    return false;
                }

                return book.Title.Contains(query.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase);
            }));
        }

        private string UploadBookImage(BookRequestModel book)
        {
            long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            var filename = $"{timestamp}.png";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage\\BookPortrait");
            var filePath = Path.Combine(directoryPath, filename);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (Image image = Image.Load(book.Image))
            {
                image.Mutate(x => x.Resize(new Size(200, 300)));
                image.Save(filePath);
            }

            return filename;
        }
    }
}
