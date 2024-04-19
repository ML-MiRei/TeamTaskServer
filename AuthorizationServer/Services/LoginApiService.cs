using AuthorizationServer;
using AuthorizationServer.Data.DataContext;
using AuthorizationServer.Data.Model;
using Grpc.Core;

namespace AuthorizationServer.Services
{
    public class LoginApiService : LoginService.LoginServiceBase, IDisposable
    {
        private static readonly MsSqlContext _msSqlContext = new MsSqlContext();


        public async override Task<AuthorizationReply> Authorization(AuthorizationRequest request, ServerCallContext context)
        {
            try
            {
                LoginData loginData = _msSqlContext.LoginData.First(l => l.Email == request.Email && l.Password == request.Password);
                return new AuthorizationReply() { IdUser = loginData.IdUser };
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
                await _msSqlContext.LoginData.AddAsync(new Data.Model.LoginData()
                {
                    Email = request.Email,
                    Password = request.Password,
                    IdUser = request.IdUser
                });
                await _msSqlContext.SaveChangesAsync();

                return new RegistrationReply();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Error database context"));
            }
        }

        public async override Task<IsRegistredReplay> IsRegistred(IsRegistredRequest request, ServerCallContext context)
        {
            return new IsRegistredReplay() { Result = _msSqlContext.LoginData.Any(l => l.Email == request.Email) };
        }

        public void Dispose()
        {
            _msSqlContext.SaveChanges();
            _msSqlContext.Dispose();
        }
    }
}
