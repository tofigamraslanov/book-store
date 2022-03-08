using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
    }
}