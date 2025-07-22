using EcommerceBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.SaleRepository.UserRepo
{
    public class SaleUserRepository : ISaleUserRepository
    {
        private readonly EcommerceDBContext _context;

        public SaleUserRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User? GetById(int id) =>
            _context.Users.FirstOrDefault(u => u.UserId == id);

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
                //_context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
