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
        void UpdateUser(UserEntity user);
        void DeleteUser(int userId);
        Task<UserEntity> GetUserById(int userId);
        Task<UserEntity> GetUserByTag(string userTag);
    }
}
