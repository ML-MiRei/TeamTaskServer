using ApiGateway.Exceptions;
using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateway.Services.AuthenticationServiceFold
{
    public class Authentication
    {

        private readonly ILogger _logger;



        private readonly AuthenticationService.AuthenticationServiceClient _serviceClient;


        public Authentication(ILogger logger)
        {
            _logger = logger;

            Health.HealthClient healthClient = new Health.HealthClient(Hosts.Connections["AuthenticationService"]);
            var response = healthClient.Check(new HealthCheckRequest());
            var status = response.Status;
            _logger.LogInformation(status.ToString());
            if (status == HealthCheckResponse.Types.ServingStatus.Serving)
            {
                _serviceClient = new AuthenticationService.AuthenticationServiceClient(Hosts.Connections["AuthenticationService"]);
            }
            else
            {
                throw new ConnectionException();
            }
        }


        public async Task<AuthenticationReply> Authenticate(AuthenticationRequest authenticationRequest)
        {
            return await _serviceClient.AuthenticationAsync(authenticationRequest);
        }

        public async Task<RegistrationReply> Registrate(RegistrationRequest registrationRequest)
        {
            return await _serviceClient.RegistrationAsync(registrationRequest);
        }

    }
}
