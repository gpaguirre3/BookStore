using Data.Models;
using Service.Models;

namespace Service.Services
{
	public interface ILoanService
	{
		Task<Loan?> GetLoanById(int id);
		Task<List<Loan>> GetAllLoans();
		Task<List<Loan>> FindLoans(Func<Loan, Boolean> callback);
		Task<bool> AddLoan(LoanRequestModel loan);
		Task<bool> UpdateLoan(LoanRequestModel loan);
	}
}
