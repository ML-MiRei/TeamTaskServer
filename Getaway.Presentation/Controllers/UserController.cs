
using Getaway.Application.CQRS.User.Commands.DeleteUser;
using Getaway.Application.CQRS.User.Commands.UpdateUser;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Getaway.Presentation.Controllers
{


    [ApiController]
    [Route("{userId}/api/[controller]")]
    public class UserController(IMediator mediator) : ControllerBase
    {



        [HttpGet("tag/{userTag}")]
        public async Task<ActionResult> GetUserByTag( string userTag)
        {
            try
            {
                Console.WriteLine("Find user..");
                var user = mediator.Send(new GetUserByTagQuery() { UserTag = userTag }).Result;
                Console.WriteLine($"{user.FirstName}");
                return Ok(user);
            }
            catch (NotFoundException)
            {
                Console.WriteLine("Not found");
                return NotFound();
            }
        }

        [HttpGet("id")]
        public async Task<ActionResult> GetUserById(int userId)
        {

            Console.WriteLine("user id = " + userId);

            try
            {
                var user = mediator.Send(new GetUserByIdQuery() { UserId = userId }).Result;
                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPatch("update")]
        public async Task<ActionResult> UpdateUser([FromBody] UserEntity user)
        {
            try
            {
                await mediator.Send(new UpdateUserCommand()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    SecondName = user.SecondName,
                    PhoneNumber = user.PhoneNumber,
                    UserId = user.ID
                });
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int userId)
        {
            try
            {
                await mediator.Send(new DeleteUserCommand() { UserId = userId });
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

    }
}
