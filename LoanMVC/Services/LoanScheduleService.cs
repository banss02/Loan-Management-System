using LoanMVC.Models;

namespace LoanMVC.Services
{
    public class LoanScheduleService
    {
        private readonly HttpClient _httpClient;

        public LoanScheduleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LoanScheduleViewModel>> GetScheduleByLoanId(int loanId)
        {
            var response = await _httpClient.GetAsync($"api/LoanSchedule/loan/{loanId}");
            if (!response.IsSuccessStatusCode) 
                return new List<LoanScheduleViewModel>();

            return await response.Content.ReadFromJsonAsync<List<LoanScheduleViewModel>>()
                   ?? new List<LoanScheduleViewModel>();
        }
    }
}
