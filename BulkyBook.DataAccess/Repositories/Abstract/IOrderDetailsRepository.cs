using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetails);
    }
}