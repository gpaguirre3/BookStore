using Business;
using Data.Models;
using Service.Models;

namespace Service.Services
{
	public class LoanService : ILoanService
	{
		private readonly ILogger<GenreService> _logger;
		private readonly LoanRepository _loanRepository;
		private readonly BookRepository _bookRepository;

		public LoanService(ILogger<GenreService> logger)
		{
			_logger = logger;
			_loanRepository = new LoanRepository();
			_bookRepository = new BookRepository();
		}

		public Task<bool> AddLoan(LoanRequestModel loan)
		{
			Book? book = _bookRepository.FindById(loan.BookId);

			if (book == null)
			{
				throw new Exception("El libro no existe o fue eliminado");
			}

			var lend = _loanRepository.FindAll(l =>
			{
				return l.Book.Id == loan.BookId
					&& l.IdentificationNumber == loan.IdentificationNumber
					&& l.Status == "PENDING";
			}).Count > 0;

			if (lend)
			{
				throw new Exception("El libro ya ha sido prestado");
			}

			Loan entity = new Loan()
			{
				IdentificationNumber = loan.IdentificationNumber,
				Email = loan.Email,
				Status = "PENDING",
				LoanedOn = DateTime.Now,
				BookId = book.Id,
			};

			return Task.Run(() => _loanRepository.Create(entity));
		}

		public Task<List<Loan>> FindLoans(Func<Loan, bool> callback)
		{
			return Task.Run(() => _loanRepository.FindAll(callback));
		}

		public Task<List<Loan>> GetAllLoans()
		{
			return Task.Run(() => _loanRepository.GetAll().OrderByDescending(x => x.Id).ToList());
		}

		public Task<Loan?> GetLoanById(int id)
		{
			return Task.Run(() => _loanRepository.FindById(id));
		}

		public async Task<bool> UpdateLoan(LoanRequestModel loan)
		{
			var id = loan.Id ?? 0;

			if (id == 0)
			{
				return false;
			}

			Loan? entity = await Task.Run(() => _loanRepository.FindById(id));

			if (entity == null)
			{
				return false;
			}

			entity.Status = loan.Status;

			return _loanRepository.Update(entity);
		}
	}
}
