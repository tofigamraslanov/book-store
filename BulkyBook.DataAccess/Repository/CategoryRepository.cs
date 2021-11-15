using System.Linq;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryFromDb = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (categoryFromDb == null) return;

            categoryFromDb.Name = category.Name;
            _context.SaveChanges();
        }
    }
}