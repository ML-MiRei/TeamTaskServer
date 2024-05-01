using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.ServicesInterfaces
{
    public interface IUserRepository
    {
        Task UpdateUser(UserEntity user);
        Task DeleteUser(int userId);
        Task<UserEntity> GetUserById(int userId);
        Task<UserEntity> GetUserByTag(string userTag);
    }
}
