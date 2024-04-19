using GreatDatabase.Data;
using GreatDatabase.Data.Model;
using Grpc.Core;

namespace AuthenticationService.Services
{
    public class AuthenticationApiService : AuthenticationService.AuthenticationServiceBase
    {
        private readonly ILogger<AuthenticationApiService> _logger;

        private MyDbContext db;


        public AuthenticationApiService(ILogger<AuthenticationApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }

        public override async Task<RegistrationReply> Registration(RegistrationRequest request, ServerCallContext context)
        {

            try
            {
                User user = new User()
                {
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.Phone,
                    SecondName = request.SecondName,
                    UserTag = GetUniqueTag(),
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now
                };


                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                _logger.LogInformation($"User {user.FirstName} with id = {user.ID} is registred");

                return new RegistrationReply()
                {
                    IdUser = user.ID,
                    Tag = user.UserTag
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Error database context"));
            }
        }

        public override async Task<AuthenticationReply> Authentication(AuthenticationRequest request, ServerCallContext context)
        {
            try
            {
                User user = db.Users.First(l => l.Email == request.Email && l.Password == request.Password);

                _logger.LogInformation($"User {user.FirstName} with id = {user.ID} is authenticated");


                return new AuthenticationReply()
                {
                    UserId = user.ID
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Not found"));
            }
        }

        private string GetUniqueTag()
        {

            while (true)
            {
                string tag = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "").Substring(0, 15);
                if (!(from u in db.Users
                      where u.UserTag == tag
                      select u).Any())
                {
                    return tag;
                }

            }
        }
    }
}
