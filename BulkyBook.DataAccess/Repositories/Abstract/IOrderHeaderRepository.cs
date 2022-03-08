using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
    }
}