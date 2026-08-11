using LoanMVC.Models;

namespace LoanMVC.Services
{
    public class DocumentService
    {
        private readonly HttpClient _httpClient;

        public DocumentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Upload(int customerId, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"api/Document/upload/{customerId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<DocumentViewModel>> GetByCustomerId(int customerId)
        {
            var response = await _httpClient.GetAsync($"api/Document/customer/{customerId}");
            if (!response.IsSuccessStatusCode)
                return new List<DocumentViewModel>();

            return await response.Content.ReadFromJsonAsync<List<DocumentViewModel>>()
                   ?? new List<DocumentViewModel>();
        }

        public async Task<List<DocumentViewModel>> GetAll()
        {
            var response = await _httpClient.GetAsync("api/Document");
            if (!response.IsSuccessStatusCode)
                return new List<DocumentViewModel>();

            return await response.Content.ReadFromJsonAsync<List<DocumentViewModel>>()
                   ?? new List<DocumentViewModel>();
        }
    }
}