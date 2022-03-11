using System.Threading.Tasks;
using BulkyBook.Entities;

namespace BulkyBook.DataAccess.Repositories.Abstract
{
    public interface ICategoryRepositoryAsync : IRepositoryAsync<Category>
    {
        Task UpdateAsync(Category category);
    }
}