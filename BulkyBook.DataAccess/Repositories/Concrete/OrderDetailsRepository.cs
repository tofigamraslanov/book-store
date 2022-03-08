using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetails orderDetails)
        {
            _context.Update(orderDetails);
        }
    }
}