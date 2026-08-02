using Microsoft.EntityFrameworkCore;
using LoanAPI.Data;
using LoanAPI.Models;

namespace LoanAPI.Repositories
{
    public class DocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Document>> GetByCustomerId(int customerId) =>
            await _context.Documents.Where(d => d.CustomerId == customerId).ToListAsync();

        public async Task<List<Document>> GetAll() =>
            await _context.Documents.ToListAsync();

        public async Task AddDocument(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }
    }
}