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

        public async Task<(bool Success, int CustomerId, string Message)> Register(RegisterCustomerViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Customer", model);

            RegisterCustomerResponseViewModel? result = null;

            try
            {
                result = await response.Content.ReadFromJsonAsync<RegisterCustomerResponseViewModel>();
            }
            catch
            {
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = result?.Message;

                if (string.IsNullOrEmpty(error))
                    error = await response.Content.ReadAsStringAsync();

                return (false, 0, error ?? "Registration failed.");
            }

            return (
                true,
                result?.CustomerId ?? 0,
                result?.Message ?? "Customer registered successfully."
            );
        }

        public async Task UploadDocument(int customerId, IFormFile file)
        {
            using var form = new MultipartFormDataContent();

            form.Add(
                new StreamContent(file.OpenReadStream()),
                "file",
                file.FileName);

            var response = await _httpClient.PostAsync(
                $"api/Document/upload/{customerId}",
                form);

            response.EnsureSuccessStatusCode();
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
                var error = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrEmpty(error) ? "Update failed." : error);
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