using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly EcommerceDBContext _context;

        public UserRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();  
        }

        public User? GetById(int id) =>
            _context.Users.FirstOrDefault(u => u.UserId == id && u.IsDelete != true);

        public void Add(User user)
        {
            user.CreateDate = DateTime.UtcNow;
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsDelete = true;
                //_context.Remove(id);
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }

}
