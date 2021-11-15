using System;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IStoredProcedureCall StoredProcedureCall { get; }
    }
}