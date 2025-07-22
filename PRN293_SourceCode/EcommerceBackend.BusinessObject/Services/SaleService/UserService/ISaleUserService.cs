using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceBackend.BusinessObject.dtos.UserDto;
using EcommerceBackend.BusinessObject.dtos.SaleDto;
namespace EcommerceBackend.BusinessObject.Services.SaleService.UserService
{
    public interface ISaleUserService
    {
        IEnumerable<User> GetAll();
        User? GetById(int id);
        void Add(UserCreateDto user);
        void Update(UserDto user);
        void Delete(int id);
    }
}
