using EcommerceBackend.BusinessObject.dtos.AuthDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.Abstract.AuthAbstract
{
    public interface IAuthService
    {
        public string GenerateJwtToken(UserDto user);
    }
}
