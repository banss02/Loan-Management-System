using LoanMVC.Models;

namespace LoanMVC.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, string Message)> Register(RegisterCustomerViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Customer", model);
            var isJson = response.Content.Headers.ContentType?.MediaType == "application/json";

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = isJson
                    ? (await response.Content.ReadFromJsonAsync<RegisterCustomerResponseViewModel>())?.Message
                    : await response.Content.ReadAsStringAsync();

                return (false, string.IsNullOrEmpty(errorMessage) ? "Registration failed." : errorMessage);
            }

            var result = isJson
                ? await response.Content.ReadFromJsonAsync<RegisterCustomerResponseViewModel>()
                : null;

            return (true, result?.Message ?? "Customer registered successfully.");
        }

        public async Task<CustomerViewModel?> GetCustomerById(int id)
        {
            var response = await _httpClient.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CustomerViewModel>();
        }

        public async Task<List<CustomerViewModel>> GetAllCustomers()
        {
            var response = await _httpClient.GetAsync("api/Customer");
            if (!response.IsSuccessStatusCode)
                return new List<CustomerViewModel>();

            return await response.Content.ReadFromJsonAsync<List<CustomerViewModel>>()
                   ?? new List<CustomerViewModel>();
        }

        public async Task<(bool Success, string Message)> UpdateCustomer(int id, UpdateCustomerViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Customer/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrEmpty(errorText) ? "Update failed." : errorText);
            }

            return (true, "Profile updated successfully.");
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Customer/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}