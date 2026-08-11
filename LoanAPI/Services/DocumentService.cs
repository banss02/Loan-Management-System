using LoanAPI.Models;
using LoanAPI.Repositories;
using LoanAPI.DTOs;
using LoanAPI.Helper;

namespace LoanAPI.Services
{
    public class DocumentService
    {
        private readonly DocumentRepository _repository;
        private readonly IWebHostEnvironment _env;
        private readonly DocumentExtractionService _documentExtractionService;
        private readonly CustomerRepository _customerRepository;
        private readonly UserRepository _userRepository;
        private readonly EncryptionService _encryptionService;

        public DocumentService(
            DocumentRepository repository,
            IWebHostEnvironment env,
            DocumentExtractionService documentExtractionService,
            CustomerRepository customerRepository,
            UserRepository userRepository,
            EncryptionService encryptionService)
        {
            _repository = repository;
            _env = env;
            _documentExtractionService = documentExtractionService;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<List<DocumentResponseDto>> GetByCustomerId(int customerId)
        {
            var docs = await _repository.GetByCustomerId(customerId);
            return docs.Select(ToDto).ToList();
        }

        public async Task<List<DocumentResponseDto>> GetAll()
        {
            var docs = await _repository.GetAll();
            return docs.Select(ToDto).ToList();
        }

        private static DocumentResponseDto ToDto(Document d) => new DocumentResponseDto
        {
            DocumentId = d.DocumentId,
            CustomerId = d.CustomerId,
            DocumentName = d.DocumentName,
            UploadedDate = d.UploadedDate
        };

        public async Task<DocumentResponseDto> Upload(
            int customerId,
            IFormFile file)
        {
            var uploadsFolder = Path.Combine(
                _env.ContentRootPath,
                "Uploads");

            Directory.CreateDirectory(uploadsFolder);

            var fileName =
                $"{customerId}_{Guid.NewGuid()}_{file.FileName}";

            var filePath =
                Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                CustomerId = customerId,
                DocumentName = file.FileName,
                FilePath = filePath,
                UploadedDate = DateTime.Now
            };

            await _repository.AddDocument(document);

            var extracted =
                await _documentExtractionService.ExtractCustomerData(file);


            var customer =
                await _customerRepository.GetCustomerById(customerId);

            if (customer != null)
            {

                var userId =
                    await _userRepository.GetUserIdByCustomerId(customerId);

                if (userId == null)
                    throw new Exception("User not found for this customer.");


                customer.CompanyName =
                    extracted.CompanyName ?? customer.CompanyName;

                customer.GuardianName =
                    extracted.GuardianName ?? customer.GuardianName;

                customer.Address =
                    extracted.Address ?? customer.Address;

                customer.BankName =
                    extracted.BankName ?? customer.BankName;


                if (!string.IsNullOrWhiteSpace(extracted.PANNumber))
                {
                    customer.PANNumber =
                        _encryptionService.Encrypt(
                            extracted.PANNumber,
                            userId.Value);
                }

                if (!string.IsNullOrWhiteSpace(extracted.AadhaarNumber))
                {
                    customer.AadhaarNumber =
                        _encryptionService.Encrypt(
                            extracted.AadhaarNumber,
                            userId.Value);
                }

                if (!string.IsNullOrWhiteSpace(extracted.AccountNumber))
                {
                    customer.AccountNumber =
                        _encryptionService.Encrypt(
                            extracted.AccountNumber,
                            userId.Value);
                }

                customer.IFSCCode =
                    extracted.IFSCCode ?? customer.IFSCCode;

                await _customerRepository.UpdateCustomer(customer);
            }

            return new DocumentResponseDto
            {
                DocumentId = document.DocumentId,
                CustomerId = document.CustomerId,
                DocumentName = document.DocumentName,
                UploadedDate = document.UploadedDate
            };
        }
    }
}