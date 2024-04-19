using Getaway.Application.CQRS.ProjectTask.Commands.ChangeStatusProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask;
using Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask;
using Getaway.Application.CQRS.Sprint.Commands.CreateSprint;
using Getaway.Application.CQRS.Sprint.Commands.DeleteSprint;
using Getaway.Application.CQRS.Sprint.Commands.UpdateDateEndSprint;
using Getaway.Application.CQRS.Sprint.Commands.UpdateDateStartSprint;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Getaway.Presentation.Controllers
{
    [Route("{userId}/api/[controller]")]
    [ApiController]
    public class SprintController(IMediator mediator) : ControllerBase
    {

        [HttpPatch("{sprintId}/update-date-start")]
        public async Task<ActionResult> UpdateDateStartSprint(int sprintId, [FromBody] DateTime dateStart)
        {
            try
            {
                await mediator.Send(new UpdateDateStartSprintCommand() { SprintId = sprintId, DateStart = dateStart });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }



        [HttpPatch("{sprintId}/update-date-end")]
        public async Task<ActionResult> UpdateDateEndSprint(int sprintId, [FromBody] DateTime dateEnd)
        {
            try
            {
                await mediator.Send(new UpdateDateEndSprintCommand() { SprintId = sprintId, DateEnd = dateEnd });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }




        [HttpPost("create")]
        public async Task<ActionResult<SprintModel>> CreateSprint([FromBody] SprintEntity sprintEntity)
        {
            try
            {
                var sprint = await mediator.Send(new CreateSprintCommand() { ProjectId = sprintEntity.ProjectId, DateEnd = sprintEntity.DateEnd, DateStart = sprintEntity.DateStart });
                return Ok(new SprintModel
                {
                    DateEnd = sprint.DateEnd,
                    DateStart = sprint.DateStart,
                    SprintId = sprint.ID,
                    Tasks = new List<ProjectTaskModel>()
                }) ;
            }
            catch
            {
                throw new Exception();
            }
        }


        [HttpDelete("{sprintId}/delete")]
        public async Task<ActionResult> DeleteSprint(int sprintId)
        {
            try
            {
                await mediator.Send(new DeleteSprintCommand() { SprintId = sprintId });
                return Ok();
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
