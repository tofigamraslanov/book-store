using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repositories.Abstract;

namespace BulkyBook.DataAccess.Repositories.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepositoryAsync = new CategoryRepositoryAsync(_context);
            CoverTypeRepository = new CoverTypeRepository(_context);
            ProductRepository = new ProductRepository(_context);
            CompanyRepository = new CompanyRepository(_context);
            ApplicationUserRepository = new ApplicationUserRepository(_context);
            ShoppingCartRepository = new ShoppingCartRepository(_context);
            OrderHeaderRepository = new OrderHeaderRepository(_context);
            OrderDetailsRepository = new OrderDetailsRepository(_context);
            StoredProcedureCall = new StoredProcedureCall(_context);
        }

        public ICategoryRepositoryAsync CategoryRepositoryAsync { get; }
        public IProductRepository ProductRepository { get; }
        public ICompanyRepository CompanyRepository { get; }
        public ICoverTypeRepository CoverTypeRepository { get; }
        public IApplicationUserRepository ApplicationUserRepository { get; }
        public IShoppingCartRepository ShoppingCartRepository { get; }
        public IOrderHeaderRepository OrderHeaderRepository { get; }
        public IOrderDetailsRepository OrderDetailsRepository { get; }
        public IStoredProcedureCall StoredProcedureCall { get; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}