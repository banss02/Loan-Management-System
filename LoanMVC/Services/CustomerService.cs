using LoanMVC.Models;
using System.Text.Json;

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

            RegisterCustomerResponseViewModel? result = null;
            try
            {
                result = await response.Content.ReadFromJsonAsync<RegisterCustomerResponseViewModel>();
            }
            catch
            {
                // API might return a plain string error - ignore parse failure, fall back below
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorText = result?.Message;
                if (string.IsNullOrEmpty(errorText))
                    errorText = await response.Content.ReadAsStringAsync();

                return (false, string.IsNullOrEmpty(errorText) ? "Registration failed." : errorText);
            }

            return (true, result?.Message ?? "Customer registered successfully.");
        }

        public async Task<CustomerViewModel?> GetCustomerById(int id)
        {
            var response = await _httpClient.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CustomerViewModel>();
        }

        public async Task<List<AdminCustomerViewModel>> GetAllCustomers()
        {
            var response = await _httpClient.GetAsync("api/Customer");
            if (!response.IsSuccessStatusCode)
                return new List<AdminCustomerViewModel>();

            return await response.Content.ReadFromJsonAsync<List<AdminCustomerViewModel>>()
                   ?? new List<AdminCustomerViewModel>();
        }

        public async Task<(bool Success, string Message)> UpdateCustomer(int id, UpdateCustomerViewModel model)
        {
           var response = await _httpClient.PutAsJsonAsync($"api/Customer/{id}", model);

           if (!response.IsSuccessStatusCode)
           {
           try
           {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            if (error != null && error.TryGetValue("message", out var message))
                return (false, message);
           }
           catch
           {
            // Ignore parse errors
           }
           return (false, "Update failed.");
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