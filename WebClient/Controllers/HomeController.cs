using Microsoft.AspNetCore.Mvc;
using PresentationWeb.ApiService;
using PresentationWeb.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WebClient.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookstoreApiService bookstoreApiService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            bookstoreApiService = new BookstoreApiService("http://localhost:5277", new HttpClient());
        }

        public async Task<IActionResult> Index()
        {
            var books = await bookstoreApiService.GetBooksAsync();
            var pendingLoans = await bookstoreApiService.SearchLoansAsync(null, "", "PENDING");
            List<BookListItem> bookList = new List<BookListItem>();

            foreach (var book in books)
            {
                bookList.Add(new BookListItem()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    Year = book.Year,
                    Publisher = book.Publisher,
                    Price = book.Price,
                    Author = book.Author,
                    Genres = book.Genres,
                    Image = book.Image,
                    IsLend = pendingLoans.Any(l => l.Book.Id == book.Id)
                });
            }


            return View("Index", new ListBooksViewModel() { Books = bookList });
        }

        public async Task<IActionResult> EditBook(int id)
		{
            Book? result;
            ICollection<Author> authors = [];
			ICollection<Genre> genres = [];

            try
            {
                result = await bookstoreApiService.GetBookAsync(id);
                authors = await bookstoreApiService.GetAuthorsAsync();
                genres = await bookstoreApiService.GetGenresAsync();
                Console.WriteLine($"libro: {result.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result = null;
            }

            return View("BookForm", new BookFormViewModel() {
				BookContext = result,
				Authors = authors.ToList(),
				Genres = genres.ToList()
			});
        }

        public async Task<IActionResult> SaveBook([FromForm] BookFormModel data)
        {
            var errors = new List<string>();

            if (data.Title == null || data.Title.Trim().Length == 0)
            {
                errors.Add("El título es necesario");
            }

            if (data.Isbn == null || data.Isbn.Trim().Length == 0)
            {
                errors.Add("El código ISBN es necesario");
            }

            // regex validation ISBN
            if (data.Isbn != null && !Regex.Match(data.Isbn, "^(?=(?:\\D*\\d){10}(?:(?:\\D*\\d){3})?$)[\\d-]+$").Success)
			{
				errors.Add("El código ISBN es incorrecto");
			}

            if (data.Year <= 0)
            {
                errors.Add("El año es necesario y debe ser mayor a 0");
            }

            if (data.Publisher == null || data.Publisher.Trim().Length == 0)
            {
                errors.Add("La editorial es necesaria");
            }

            if (data.Price < 0)
            {
                errors.Add("El precio debe ser mayor o igual a cero");
            }

            if (data.AuthorId == 0)
            {
                errors.Add("El autor es requerido");
            }

            if (data.GenreIds.Count == 0)
            {
                errors.Add("Se necesita al menos un g nero para el libro");
            }

			byte[] imageBytes = [];

			if (data.Image != null && data.Image.Length > 0)
			{
				using (var ms = new MemoryStream())
				{
                    if (data.Image.ContentType != "image/jpeg" 
                        && data.Image.ContentType != "image/png" 
                        && data.Image.ContentType != "image/gif" 
                        && data.Image.ContentType != "image/webp")
					{
						errors.Add("El formato de la imagen no es válido");
					}
                    else
                    {
						data.Image.CopyTo(ms);
						imageBytes = ms.ToArray();
					}
				}
			}

			if (errors.Count > 0)
            {
                var authors = await bookstoreApiService.GetAuthorsAsync();
                var genres = await bookstoreApiService.GetGenresAsync();
                /*string? tempImage = null;

                if (imageBytes.Length > 0)
                {
                    var base64 = Convert.ToBase64String(imageBytes);
                    var extension = base64[0] switch
                    {
                        '/' => "jpg",
                        'i' => "png",
                        'R' => "gif",
                        'U' => "webp",
                        _ => "jpg"
                    };

                    tempImage = $"data:image/{extension};base64,{base64}";
                }*/

				return View("BookForm", new BookFormViewModel()
                {
                    BookContext = new Book()
                    {
                        Id = data.Id ?? 0,
                        Title = data.Title ?? "",
                        Isbn = data.Isbn ?? "",
                        Year = data.Year,
                        Publisher = data.Publisher ?? "",
                        Price = data.Price,
                        Author = new Author() { Id = data.AuthorId },
                        Genres = data.GenreIds.Select(id => new Genre() { Id = id }).ToList(),
                        Image = data.ServerImage != null ? data.ServerImage : ""
                    },
                    Authors = authors.ToList(),
                    Genres = genres.ToList(),
                    Errors = errors.ToList()
                });
            }

            BookRequestModel book = new BookRequestModel()
            {
                Id = data.Id ?? 0,
                Title = data.Title,
                Isbn = data.Isbn,
                Year = data.Year,
                Publisher = data.Publisher,
                Price = data.Price,
                Author = new AuthorRequestModel() { Id = data.AuthorId },
                Genres = data.GenreIds.Select(id => new GenreRequestModel() { Id = id }).ToList(),
                Image = imageBytes
			};

            if (data.Id.HasValue)
            {
                await bookstoreApiService.UpdateBookAsync(book);
            }
            else
            {
                await bookstoreApiService.AddBookAsync(book);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddBook()
        {
			ICollection<Author> authors = [];
			ICollection<Genre> genres = [];

			try
			{
				authors = await bookstoreApiService.GetAuthorsAsync();
				genres = await bookstoreApiService.GetGenresAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return View("BookForm", new BookFormViewModel()
			{
				Authors = authors.ToList(),
				Genres = genres.ToList()
			});
		}

        public async Task<IActionResult> DeleteBook(int id)
        {
            await bookstoreApiService.DeleteLoansByBookIdAsync(id);
			await bookstoreApiService.DeleteBookAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Search([FromQuery] string? query)
        {
            List<Book> books = [];

            if (query != null && query.Trim().Length == 0)
            {
                return View();
            }

            if (query != null)
            {
                var result = await bookstoreApiService.SearchBooksAsync(query);
                books = result.ToList();
            }

            return View("Search", new SearchBooksViewModel()
            {
                Books = books,
                Query = query ?? ""
            });
        }

        public async Task<IActionResult> ShowLoans()
        {
            var loans = await bookstoreApiService.GetLoansAsync();

            return View("Loans", new ListLoansViewModel()
            {
                Loans = loans.ToList(),
            });
        }

        public async Task<IActionResult> LendBook(int id)
        {
            var book = await bookstoreApiService.GetBookAsync(id);

            if (book == null)
            {
                return RedirectToAction("Index");
            }

            return View("LendBook", new LendBookViewModel() { TargetBook = book });
        }

        public async Task<IActionResult> DoLendBook([FromForm] LendFormModel form)
        {
            List<string> errors = [];

            if (form.Email == null || form.Email.Trim().Length == 0)
            {
                errors.Add("El email es necesario");
            }

            if (form.IdentificationNumber == null || form.IdentificationNumber.Trim().Length == 0)
            {
                errors.Add("La cédula es necesaria");
            } 
            else if (form.IdentificationNumber != null && !Regex.Match(form.IdentificationNumber, "^[0-9]{10}$").Success)
            {
				errors.Add("La cédula debe tener 10 digitos numéricos");
			}

            if (errors.Count > 0)
            {
                var book = await bookstoreApiService.GetBookAsync(form.BookId);

                return View("LendBook", new LendBookViewModel {
                    TargetBook = book,
                    Errors = errors,
                    IdentificationNumber = form.IdentificationNumber,
                    Email = form.Email
                });
            }

            LoanRequestModel loan = new LoanRequestModel()
            {
                BookId = form.BookId,
                IdentificationNumber = form.IdentificationNumber!.Trim(),
                Email = form.Email!.Trim(),
                Status = "PENDING"
            };

            await bookstoreApiService.AddLoanAsync(loan);
            return RedirectToAction("ShowLoans");
        }

        public async Task<IActionResult> CompleteLoan(int id)
        {
            var loan = bookstoreApiService.GetLoanAsync(id);

            if (loan == null)
            {
                return RedirectToAction("ShowLoans");
            }

            var request = new LoanRequestModel()
            {
                Id = id,
                Status = "RETURNED",
            };

            await bookstoreApiService.UpdateLoanAsync(request);
            return RedirectToAction("ShowLoans");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
