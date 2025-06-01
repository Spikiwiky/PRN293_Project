using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.DataAccess.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly EcommerceDBContext _context;
        public UserRepository(EcommerceDBContext context)
        {
            _context = context;
        }

        public IEnumerable<EcommerceBackend.DataAccess.Models.User> GetAll() => _context.Users.ToList();

        public EcommerceBackend.DataAccess.Models.User GetById(string id) => _context.Users.Find(id);

        public void Create(EcommerceBackend.DataAccess.Models.User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(EcommerceBackend.DataAccess.Models.User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
