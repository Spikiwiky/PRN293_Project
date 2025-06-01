using EcommerceBackend.DataAccess.Repository.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.User
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _repo;
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<EcommerceBackend.DataAccess.Models.User> GetAll() => _repo.GetAll();
        public EcommerceBackend.DataAccess.Models.User GetById(string id) => _repo.GetById(id);
        public void Create(EcommerceBackend.DataAccess.Models.User user) => _repo.Create(user);
        public void Update(EcommerceBackend.DataAccess.Models.User user) => _repo.Update(user);
        public void Delete(string id) => _repo.Delete(id);
    }
}
