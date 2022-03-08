using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}