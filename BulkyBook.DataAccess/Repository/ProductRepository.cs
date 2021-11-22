using System.Linq;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productFromDb = _context.Products.FirstOrDefault(p => p.Id == product.Id);

            if (productFromDb == null) return;

            if (product.ImageUrl != null)
                productFromDb.ImageUrl = product.ImageUrl;

            productFromDb.Title = product.Title;
            productFromDb.Description = product.Description;
            productFromDb.Isbn = product.Isbn;
            productFromDb.Author = product.Author;
            productFromDb.ListPrice = product.ListPrice;
            productFromDb.Price = product.Price;
            productFromDb.Price50 = product.Price50;
            productFromDb.Price100 = product.Price100;
            productFromDb.CategoryId = product.CategoryId;
            productFromDb.CoverTypeId = product.CoverTypeId;
        }
    }
}