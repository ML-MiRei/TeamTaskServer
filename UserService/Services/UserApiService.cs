using GreatDatabase.Data;
using Grpc.Core;

namespace UserService.Services
{
    public class UserApiService : UserService.UserServiceBase
    {
        private readonly ILogger<UserApiService> _logger;
        private readonly MyDbContext db;
        public UserApiService(ILogger<UserApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }

        public async override Task<UserVoidReply> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var user = await db.Users.FindAsync(request.UserId);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            try
            {
                db.Users.Remove(user);
                db.SaveChanges();

                _logger.LogInformation($"Delete user with id = {request.UserId}");

                return await Task.FromResult(new UserVoidReply());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Delete element of database error"));
            }
        }

        public async override Task<GetUserReply> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var user = (from u in db.Users
                        where u.ID == request.UserId
                        select new GetUserReply
                        {
                            FirstName = u.FirstName,
                            SecondName = u.SecondName,
                            LastName = u.LastName,
                            PhoneNumber = u.PhoneNumber,
                            Email = u.Email,
                            Tag = u.UserTag
                        }).FirstOrDefault();
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            _logger.LogInformation($"return user with tag = {user.Tag}");

            return await Task.FromResult(user);
        }

        public async override Task<GetUserReply> GetUserByTag(GetUserByTagRequest request, ServerCallContext context)
        {
            var user = (from u in db.Users
                        where u.UserTag == request.Tag
                        select new GetUserReply
                        {
                            FirstName = u.FirstName,
                            SecondName = u.SecondName,
                            LastName = u.LastName,
                            PhoneNumber = u.PhoneNumber,
                            Email = u.Email,
                            Tag = u.UserTag,
                            Id = u.ID
                        }).FirstOrDefault();
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            _logger.LogInformation($"return user with tag = {user.Tag}");

            return await Task.FromResult(user);
        }


        public async override Task<UserVoidReply> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {

            var user = await db.Users.FindAsync(request.Id);

            if (user == null)
            {
                _logger.LogError("User not found");
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            try
            {
                user.FirstName = String.IsNullOrEmpty(request.FirstName) ?  user.FirstName : request.FirstName;
                user.SecondName = request.SecondName ;
                user.LastName = String.IsNullOrEmpty(request.LastName) ? user.LastName : request.LastName;
                user.PhoneNumber = String.IsNullOrEmpty(request.PhoneNumber) ? user.PhoneNumber: request.PhoneNumber;
                user.LastModified = DateTime.Now;

                db.Users.Update(user);
                db.SaveChanges();
                _logger.LogInformation("Success");
                return await Task.FromResult(new UserVoidReply());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Update element of database error"));
            }
        }


    }


}
