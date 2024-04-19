using Microsoft.AspNetCore.Mvc;
using Getaway.Infrustructure.Services.Interfaces;
using Getaway.Core.Entities;
using Getaway.Application.ReturnsModels;

namespace Getaway.Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {


        [HttpGet("authenticate/email={email}&password={password}")]
        public async Task<ActionResult> Authentication(string email, string password)
        {
            try
            {
                var user = await authenticationService.Authentication(email, password);
                return Ok(user);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("registration")]
        public async Task<ActionResult> Registration([FromBody] UserEntity userEntity)
        {
            try
            {
                Console.WriteLine("start registration...");
                var user = await authenticationService.Registration(userEntity);
                Console.WriteLine("end");
                return Ok(user);
            }
            catch (Exception) 

            {
                return NotFound();
            }
        }

    }
}
