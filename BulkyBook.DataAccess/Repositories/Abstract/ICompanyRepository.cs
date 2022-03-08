using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);
    }
}