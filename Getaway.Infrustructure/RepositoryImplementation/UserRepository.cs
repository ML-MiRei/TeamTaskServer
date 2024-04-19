using Getaway.Application.ServicesInterfaces;
using Getaway.Core;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;


namespace Getaway.Infrustructure.RepositoryImplementation
{
    public class UserRepository : IUserRepository
    {
        public async void DeleteUser(int userId)
        {
            try
            {
                await Connections.UserServiceClient.DeleteUserAsync(new DeleteUserRequest() { UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }


        public async Task<UserEntity> GetUserById(int userId)
        {
            try
            {
                var user = await Connections.UserServiceClient.GetUserByIdAsync(new GetUserByIdRequest() { UserId = userId });
                return new UserEntity()
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Tag = user.Tag,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
            }
            catch
            {
                throw new NotFoundException();
            }
        }


        public async Task<UserEntity> GetUserByTag(string userTag)
        {
            try
            {
                var user = await Connections.UserServiceClient.GetUserByTagAsync(new GetUserByTagRequest() { Tag = userTag });
                return new UserEntity()
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
            }
            catch
            {
                throw new NotFoundException();
            }
        }

        public async void UpdateUser(UserEntity user)
        {
            try
            {
                await Connections.UserServiceClient.UpdateUserAsync(new UpdateUserRequest()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondName = user.SecondName,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.ID
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception();
            }

        }
    }
}
