
using Grpc.Core;
using Server.Data;
using Server.Data.Model;
using Services;

namespace Server.Services
{
    public class LoginApiService : LoginService.LoginServiceBase
    {

        MyDbContext db;
        public LoginApiService()
        {
            db = Config.myDbContext;
        }

        public async override Task<AuthorizationReply> Authorization(AuthorizationRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine(request.Email);
                Console.WriteLine(request.Password);
                User user = db.Users.First(l => l.Email == request.Email && l.Password == request.Password);
                Console.WriteLine("user id = ", user.ID);
                return new AuthorizationReply() { IdUser = user.ID, 
                Email = user.Email,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName,
                Phone =user.Phone,
                Tag = user.Tag,
                
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Not found"));
            }
        }

        public async override Task<RegistrationReply> Registration(RegistrationRequest request, ServerCallContext context)
        {
            try
            {
                User user = new User()
                {
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Phone = request.Phone,
                    SecondName = request.SecondName,
                    Tag = GetUniqueTag()
                };


                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                return new RegistrationReply()
                {
                    IdUser = user.ID,
                    Tag = user.Tag
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Error database context"));
            }
        }

        public async override Task<IsRegistredReplay> IsRegistred(IsRegistredRequest request, ServerCallContext context)
        {
            return new IsRegistredReplay() { Result = db.Users.Any(u => u.Email == request.Email)};
        }
        private string GetUniqueTag()
        {

            while (true)
            {
                string tag = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 15);
                if (!(from u in db.Users
                      where u.Tag == tag
                      select u).Any())
                {
                    return tag;
                }

            }
        }

    }
}
