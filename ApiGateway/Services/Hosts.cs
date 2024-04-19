using Grpc.Net.Client;

namespace ApiGateway.Services
{
    public static class Hosts
    {

        public static Dictionary<string, GrpcChannel> Connections = new Dictionary<string, GrpcChannel>();

        static Hosts()
        {
            Connections.Add("AuthenticationService", GrpcChannel.ForAddress("https://localhost:7245"));
        }

    }
}
