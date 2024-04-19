using Azure;
using Grpc.Core;
using Server;
using Server.Data;
using Server.Data.Model;
using Services;


namespace Server.Services
{
    public class UserApiService : UserService.UserServiceBase
    {
        MyDbContext db;
        public UserApiService()
        {
            db = Config.myDbContext;

        }

        public override Task<ListUsersReply> GetUserByChat(GetUserByChatRequest request, ServerCallContext context)
        {
            ListUsersReply listTeams = new ListUsersReply();
            List<UserListItemReply> replyList = (from u in db.Users
                                                 join cu in db.Chats_Users on u.ID equals cu.ID_User
                                                 where cu.ID_Chat == request.IdChat
                                                 select new UserListItemReply()
                                                 {
                                                     SecondName = u.SecondName,
                                                     FirstName = u.FirstName,
                                                     LastName = u.LastName,
                                                     Phone = u.Phone,
                                                     Email = u.Email,
                                                     Tag = u.Tag

                                                 }).ToList();

            listTeams.Users.AddRange(replyList);
            return Task.FromResult(listTeams);
        }

     
        public override async Task<UserReply> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var user = await db.Users.FindAsync(request.Id);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            try
            {
                db.Update(user);
                return await Task.FromResult(new UserReply()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondName = user.SecondName,
                    Phone = user.Phone,
                    IdUser = user.ID,
                    Tag = user.Tag
                });
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Update element of database error"));
            }
        }
        public override async Task<UserReply> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var user = await db.Users.FindAsync(request.Id);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            try
            {
                db.Remove(user);
                return await Task.FromResult(new UserReply()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondName = user.SecondName,
                    Phone = user.Phone,
                    IdUser = user.ID,
                    Tag = user.Tag
                });
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Delete element of database error"));
            }
        }
        public override async Task<UserReply> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var user = (from u in db.Users
                        where u.Tag == request.Tag
                        select new UserReply
                        {
                            IdUser = u.ID,
                            FirstName = u.FirstName,
                            SecondName = u.SecondName,
                            LastName = u.LastName,
                            Phone = u.Phone,
                            Email = u.Email,
                            Tag = u.Tag
                        }).FirstOrDefault();
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }
            Console.WriteLine(user.IdUser);
            return await Task.FromResult(user);
        }




        public override Task<ListUsersReply> GetUserByTeam(GetUserByTeamRequest request, ServerCallContext context)
        {
            ListUsersReply listTeams = new ListUsersReply();
            List<UserListItemReply> replyList = (from u in db.Users
                                         join tu in db.Teams_Users on u.ID equals tu.ID_User
                                         where tu.ID_Team == request.IdTeam
                                         select new UserListItemReply()
                                         {
                                             SecondName = u.SecondName,
                                             FirstName = u.FirstName,
                                             LastName = u.LastName,
                                             Phone = u.Phone,
                                             Email = u.Email,
                                             Tag = u.Tag

                                         }).ToList();

            listTeams.Users.AddRange(replyList);
            return Task.FromResult(listTeams);

        }



    }
}
