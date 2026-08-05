using LoanMVC.Models;

namespace LoanMVC.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(LoginResultViewModel? Result, string? ErrorMessage)> Login(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/login", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorViewModel>();
                return (null, string.IsNullOrEmpty(error?.Message) ? "Invalid username or password." : error.Message);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResultViewModel>();
            return (result, null);
        }

        public async Task Logout()
        {
            await _httpClient.PostAsync("api/User/logout", null);
        }
    }
}