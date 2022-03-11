using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class CategoryRepositoryAsync : RepositoryAsync<Category>, ICategoryRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepositoryAsync(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Category category)
        {
            var categoryFromDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (categoryFromDb == null) 
                return;

            categoryFromDb.Name = category.Name;
        }
    }
}