using System;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepositoryAsync CategoryRepositoryAsync { get; }
        ICoverTypeRepository CoverTypeRepository { get; }
        IProductRepository ProductRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IOrderDetailsRepository OrderDetailsRepository { get; }
        IStoredProcedureCall StoredProcedureCall { get; }

        void Save();
    }
}