using LoanMVC.Models;

namespace LoanMVC.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> MakePayment(PaymentViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Payment", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PaymentViewModel>> GetPayments()
        {
            var response = await _httpClient.GetAsync("api/Payment");
            if (!response.IsSuccessStatusCode)
                return new List<PaymentViewModel>();

            return await response.Content.ReadFromJsonAsync<List<PaymentViewModel>>()
                   ?? new List<PaymentViewModel>();
        }

        public async Task<List<PaymentViewModel>> GetPaymentsByCustomerId(int customerId)
        {
            var response = await _httpClient.GetAsync($"api/Payment/customer/{customerId}");
            if (!response.IsSuccessStatusCode)
                return new List<PaymentViewModel>();

            return await response.Content.ReadFromJsonAsync<List<PaymentViewModel>>()
                   ?? new List<PaymentViewModel>();
        }
    }
}
