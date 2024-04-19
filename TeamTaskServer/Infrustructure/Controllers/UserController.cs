using ApiGetaway.Core.ConnectionSettings;
using ApiGetaway.Core.Exceptions;
using ApiGetaway.Logic.UserLogic.Commands.DeleteUser;
using ApiGetaway.Logic.UserLogic.Commands.UpdateUser;
using ApiGetaway.Logic.UserLogic.Queries.GetUserById;
using Grpc.Net.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary;
using ModelsLibrary.Entities;
using TeamTaskServerAPI;

namespace ApiGetaway.Infrustructure.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IMediator mediator) : ControllerBase
    {



        [HttpGet("get-user-by-tag/user-tag={userTag}")]
        public async Task<ActionResult> GetUserByTag(string userTag)
        {
            try
            {
                var user = mediator.Send(new GetUserByTagQuery() { UserTag = userTag }).Result;
                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("get-user-by-id/user-id={userId}")]
        public async Task<ActionResult> GetUserById(int userId)
        {
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


        [HttpPatch("update-user/{user}")]
        public async Task<ActionResult> UpdateUser(User user)
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


        [HttpDelete("delete-user/{userId}")]
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
