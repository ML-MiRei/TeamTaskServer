using Getaway.Core.Entities;
using Getaway.Infrustructure.Services.Interfaces;
using Grpc.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.Services.Implementations
{
    public class AuthenticationServiceImpl : IAuthenticationService
    {
        public async Task<UserEntity> Authentication(string email, string password)
        {
            try
            {
                var reply = await Connections.AuthenticationServiceClient.AuthenticationAsync(new AuthenticationRequest() { Email = email, Password = password });
                var user = await Connections.UserServiceClient.GetUserByIdAsync(new GetUserByIdRequest() { UserId = reply.UserId });
                return new UserEntity
                {
                    Email = email,
                    Tag = user.Tag,
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ID = reply.UserId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Not found"));
            }
        }

        public async Task<UserEntity> Registration(UserEntity userEntity)
        {
            try
            {
                var reply = await Connections.AuthenticationServiceClient.RegistrationAsync(new RegistrationRequest()
                {
                    Email = userEntity.Email,
                    Password = userEntity.Password,
                    FirstName = userEntity.FirstName,
                    SecondName = userEntity.SecondName,
                    LastName = userEntity.LastName,
                    Phone = userEntity.PhoneNumber                    
                });


                var user = await Connections.UserServiceClient.GetUserByIdAsync(new GetUserByIdRequest() { UserId = reply.IdUser });

                Console.WriteLine(user.Tag);

                return new UserEntity
                {
                    Email = userEntity.Email,
                    Tag = user.Tag,
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ID = reply.IdUser
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Not found"));
            }
        }

    }
}
