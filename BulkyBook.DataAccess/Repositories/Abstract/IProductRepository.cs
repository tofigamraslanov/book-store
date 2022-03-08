using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}