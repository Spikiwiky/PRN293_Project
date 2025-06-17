using EcommerceBackend.BusinessObject.dtos.BlogDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Abstract.BlogAbstract
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetAllAsync();
        Task<BlogDto> GetByIdAsync(int id);
        Task AddAsync(BlogDto dto);              
        Task UpdateAsync(BlogDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<BlogDto>> LoadBlogsAsync(int page, int pageSize);
    }
}
