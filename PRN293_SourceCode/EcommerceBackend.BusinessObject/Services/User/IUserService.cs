using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Services.User
{
    public interface IUserService
    {
        IEnumerable<EcommerceBackend.DataAccess.Models.User> GetAll();
        EcommerceBackend.DataAccess.Models.User GetById(string id);
        void Create(EcommerceBackend.DataAccess.Models.User user);
        void Update(EcommerceBackend.DataAccess.Models.User user);
        void Delete(string id);
    }
}
