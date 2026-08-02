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

        public async Task<LoginResultViewModel?> Login(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/login", model);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResultViewModel>();
        }
    }
}
