using EcommerceBackend.BusinessObject.dtos.UserDto;
using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.UserService
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        User? GetById(int id);
        void Add(UserDto user);
        void Update(UserDto user);
        void Delete(int id);
    }

}
