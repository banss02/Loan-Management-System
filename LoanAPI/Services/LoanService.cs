using LoanAPI.Models;
using LoanAPI.Repositories;
using LoanAPI.DTOs;

namespace LoanAPI.Services
{
    public class LoanService
    {
        private readonly LoanRepository _loanRepository;
        private readonly LoanScheduleRepository _scheduleRepository;

        public LoanService(LoanRepository loanRepository, LoanScheduleRepository scheduleRepository)
        {
            _loanRepository = loanRepository;
            _scheduleRepository = scheduleRepository;
        }

        private static decimal GetInterestRate(string loanType) => 
        loanType.ToLower() switch
        {
            "home" => 8.0m,
            "car" => 10.0m,
            "education" => 7.0m,
            "personal" => 12.0m,
            _ => 10.0m
        };

        public async Task<List<LoanResponseDto>> GetAllLoans()
        {
            var loans = await _loanRepository.GetAllLoans();
            return loans.Select(ToDto).ToList();
        }

        public async Task<List<LoanResponseDto>> GetLoansByCustomerId(int customerId)
        {
            var loans = await _loanRepository.GetLoansByCustomerId(customerId);
            return loans.Select(ToDto).ToList();
        }

        public async Task<LoanResponseDto?> GetLoanById(int id)
        {
            var loan = await _loanRepository.GetLoanById(id);
            return loan == null ? null : ToDto(loan);
        }

        // Raw entity - used internally by controllers that need to check ownership (loan.CustomerId)
        public async Task<Loan?> GetLoanEntityById(int id) => await _loanRepository.GetLoanById(id);

        public async Task<(bool Success, string Message)> ApplyLoan(ApplyLoanDto dto)
        {
            if (dto.LoanAmount < 10000)
                return (false, "Loan amount must be at least 10000.");

            if (dto.TenureMonths < 1 || dto.TenureMonths > 360)
                return (false, "Tenure must be between 1 and 360 months.");

            var loan = new Loan
            {
                CustomerId = dto.CustomerId,
                LoanType = dto.LoanType,
                LoanAmount = dto.LoanAmount,
                TenureMonths = dto.TenureMonths,
                InterestRate = GetInterestRate(dto.LoanType),
                Status = "Pending",
                AppliedDate = DateTime.Now
            };

            await _loanRepository.AddLoan(loan);
            return (true, "Loan application submitted.");
        }

        public async Task<bool> ApproveLoan(int id)
        {
            var loan = await _loanRepository.GetLoanById(id);
            if (loan == null || loan.Status != "Pending")
                return false;

            loan.Status = "Approved";
            await _loanRepository.UpdateLoan(loan);

            await GenerateSchedule(loan);
            return true;
        }

        public async Task<bool> RejectLoan(int id)
        {
            var loan = await _loanRepository.GetLoanById(id);
            if (loan == null || loan.Status != "Pending")
                return false;

            loan.Status = "Rejected";
            await _loanRepository.UpdateLoan(loan);
            return true;
        }

        // Standard reducing-balance EMI schedule
        private async Task GenerateSchedule(Loan loan)
        {
            decimal monthlyRate = loan.InterestRate / 12 / 100;
            int n = loan.TenureMonths;
            decimal principal = loan.LoanAmount;

            decimal emi;
            if (monthlyRate == 0)
            {
                emi = Math.Round(principal / n, 2);
            }
            else
            {
                double r = (double)monthlyRate;
                double factor = Math.Pow(1 + r, n);
                emi = Math.Round(principal * (decimal)(r * factor / (factor - 1)), 2);
            }

            var schedules = new List<LoanSchedule>();
            decimal outstanding = principal;

            for (int i = 1; i <= n; i++)
            {
                decimal interestPart = Math.Round(outstanding * monthlyRate, 2);
                decimal principalPart = (i == n) ? outstanding : emi - interestPart;
                outstanding -= principalPart;

                schedules.Add(new LoanSchedule
                {
                    LoanId = loan.LoanId,
                    InstallmentNo = i,
                    DueDate = loan.AppliedDate.AddMonths(i),
                    EMIAmount = (i == n) ? principalPart + interestPart : emi,
                    PrincipalAmount = principalPart,
                    InterestAmount = interestPart,
                    IsPaid = false
                });
            }

            await _scheduleRepository.AddRange(schedules);
        }

        private static LoanResponseDto ToDto(Loan loan) => new LoanResponseDto
        {
            LoanId = loan.LoanId,
            CustomerId = loan.CustomerId,
            LoanType = loan.LoanType,
            LoanAmount = loan.LoanAmount,
            TenureMonths = loan.TenureMonths,
            InterestRate = loan.InterestRate,
            Status = loan.Status,
            AppliedDate = loan.AppliedDate
        };
    }
}
