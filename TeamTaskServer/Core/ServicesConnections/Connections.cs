using ApiGetaway.Core.Exceptions;
using Grpc.Health.V1;
using Grpc.Net.Client;

namespace ApiGetaway.Core.ConnectionSettings
{
    public static class Connections
    {

        private static AuthenticationService.AuthenticationServiceClient _authenticationServiceClient;
        private static UserService.UserServiceClient _userServiceClient;
        private static TeamService.TeamServiceClient _teamServiceClient;

        public static AuthenticationService.AuthenticationServiceClient AuthenticationServiceClient
        {
            get
            {
                if (_authenticationServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7245");
                        _authenticationServiceClient = new AuthenticationService.AuthenticationServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _authenticationServiceClient;
            }
        }

        public static UserService.UserServiceClient UserServiceClient
        {
            get
            {
                if (_userServiceClient == null)
                {
                    _userServiceClient = new UserService.UserServiceClient(GrpcChannel.ForAddress("https://localhost:7260"));
                }
                return _userServiceClient;
            }
        }

        public static TeamService.TeamServiceClient TeamServiceClient
        {
            get
            {
                if (_teamServiceClient == null)
                {
                    try
                    {
                        var channel = GrpcChannel.ForAddress("https://localhost:7024");
                        _teamServiceClient = new TeamService.TeamServiceClient(channel);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ConnectionServiceException();
                    }

                }
                return _teamServiceClient;
            }
        }
    }
}
