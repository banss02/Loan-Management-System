using LoanAPI.DTOs;
using LoanAPI.Repositories;

namespace LoanAPI.Services
{
    public class LoanScheduleService
    {
        private readonly LoanScheduleRepository _repository;

        public LoanScheduleService(LoanScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<LoanScheduleResponseDto>> GetByLoanId(int loanId)
        {
            var schedules = await _repository.GetByLoanId(loanId);

            return schedules.Select(s => new LoanScheduleResponseDto
            {
                ScheduleId = s.ScheduleId,
                LoanId = s.LoanId,
                InstallmentNo = s.InstallmentNo,
                DueDate = s.DueDate,
                EMIAmount = s.EMIAmount,
                PrincipalAmount = s.PrincipalAmount,
                InterestAmount = s.InterestAmount,
                IsPaid = s.IsPaid
            }).ToList();
        }
    }
}
