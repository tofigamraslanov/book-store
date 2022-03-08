using System.Linq;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(CoverType coverType)
        {
            var coverTypeFromDb = _context.CoverTypes.FirstOrDefault(c => c.Id == coverType.Id);
            if (coverTypeFromDb == null)
                return;

            coverTypeFromDb.Name = coverType.Name;
        }
    }
}