using ApiGetaway.Core.ConnectionSettings;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Logic.UserLogic.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Entities;

namespace ApiGetaway.Infrustructure.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {

        private static AuthenticationService.AuthenticationServiceClient _authenticationServiceClient;
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            try
            {
                _authenticationServiceClient = Connections.AuthenticationServiceClient;
                _mediator = mediator;
            }
            catch (ConnectionServiceException ex)
            {
                Console.WriteLine(ex.Message);
                throw new ConnectionServiceException();
            }


        }




        [HttpGet("authenticate/email={email}&password={password}")]
        public async Task<ActionResult> Authentication(string email, string password)
        {
            try
            {
                var userId = await _authenticationServiceClient.AuthenticationAsync(new AuthenticationRequest() { Email = email, Password = password });
                var user = await _mediator.Send(new GetUserByIdQuery() { UserId = userId.UserId });
                return Ok(user);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpPost("registrate/{user}")]
        public async Task<ActionResult> Registration(User user)
        {
            try
            {
                var newUser = await _authenticationServiceClient.RegistrationAsync(new RegistrationRequest()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondName = user.SecondName,
                    Password = user.Password,
                    Phone = user.PhoneNumber

                });

                return Ok(new JsonResult(newUser));
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}
