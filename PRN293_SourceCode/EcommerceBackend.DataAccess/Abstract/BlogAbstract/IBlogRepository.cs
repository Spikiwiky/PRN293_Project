using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract.BlogAbstract
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllAsync();
        Task<Blog> GetByIdAsync(int id);
        Task AddAsync(Blog blog);
        Task<IEnumerable<Blog>> GetPagedAsync(int page, int pageSize);
        Task UpdateAsync(Blog blog);
        Task DeleteAsync(int id);
     
    }
}
