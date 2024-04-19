using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserEntity> Authentication(string email, string password);
        Task<UserEntity> Registration(UserEntity userEntity);

    }
}
