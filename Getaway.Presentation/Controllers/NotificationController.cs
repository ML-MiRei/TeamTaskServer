using Getaway.Application.CQRS.Notification.Commands.GetNotifications;
using Getaway.Application.CQRS.Project.Queries.GetProjects;
using Getaway.Application.CQRS.Project.Queries.GetUsersByProject;
using Getaway.Application.CQRS.Sprint.Commands.GetSprints;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Getaway.Presentation.Controllers
{

    [Route("{userId}/api/[controller]")]
    [ApiController]
    public class NotificationController(IMediator mediator) : ControllerBase
    {

        [HttpGet("list")]
        public async Task<ActionResult<List<NotificationModel>>> GetNotifications(int userId)
        {
            try
            {

                var notifications = await mediator.Send(new GetNotificationsQuery() { UserId = userId });

                return Ok(notifications.Select(n => new NotificationModel
                {
                    Details = n.Detail,
                    Title = n.Title,
                    NotificationId = n.ID
                }));

            }
            catch
            {
                throw new Exception();
            }
        }


    }
}
