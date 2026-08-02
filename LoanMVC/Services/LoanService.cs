using LoanMVC.Models;

namespace LoanMVC.Services
{
    public class LoanService
    {
        private readonly HttpClient _httpClient;

        public LoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LoanViewModel>> GetLoans()
        {
            var response = await _httpClient.GetAsync("api/Loan");
            if (!response.IsSuccessStatusCode)
                return new List<LoanViewModel>();

            return await response.Content.ReadFromJsonAsync<List<LoanViewModel>>()
                   ?? new List<LoanViewModel>();
        }

        public async Task<List<LoanViewModel>> GetLoansByCustomerId(int customerId)
        {
            var response = await _httpClient.GetAsync($"api/Loan/customer/{customerId}");
            if (!response.IsSuccessStatusCode)
                return new List<LoanViewModel>();

            return await response.Content.ReadFromJsonAsync<List<LoanViewModel>>()
                   ?? new List<LoanViewModel>();
        }

        public async Task<LoanViewModel?> GetLoanById(int id)
        {
            var response = await _httpClient.GetAsync($"api/Loan/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoanViewModel>();
        }

        public async Task<(bool Success, string Message)> ApplyLoan(ApplyLoanViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Loan", model);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrEmpty(errorText) ? "Loan application failed." : errorText);
            }

            return (true, "Loan application submitted.");
        }

        public async Task<bool> ApproveLoan(int id)
        {
            var response = await _httpClient.PutAsync($"api/Loan/{id}/approve", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectLoan(int id)
        {
            var response = await _httpClient.PutAsync($"api/Loan/{id}/reject", null);
            return response.IsSuccessStatusCode;
        }
    }
}
