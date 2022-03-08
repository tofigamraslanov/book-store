using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _context.Update(shoppingCart);  
        }
    }
}