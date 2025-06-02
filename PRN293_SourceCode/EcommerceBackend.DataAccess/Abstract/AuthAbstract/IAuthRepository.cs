using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Abstract.AuthAbstract
{
    public interface IAuthRepository
    {
        User? GetUserByEmail(string email);
        Task<User?> GetUserByIdAsync(int userId);
        User? CreateUser(User user);
    }
}
