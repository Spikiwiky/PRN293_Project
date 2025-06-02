using EcommerceBackend.DataAccess.Abstract.AuthAbstract;
using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EcommerceDBContext _context;

        public AuthRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public EcommerceBackend.DataAccess.Models.User? GetUserByEmail(string email)
        {
            try
            {
                return _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Email == email);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<EcommerceBackend.DataAccess.Models.User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId && (u.IsDelete == null || u.IsDelete == false));
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
