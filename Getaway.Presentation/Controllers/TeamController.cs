
using Getaway.Application.CQRS.Team.Commands.AddUserInTeam;
using Getaway.Application.CQRS.Team.Commands.CreateTeam;
using Getaway.Application.CQRS.Team.Commands.DeleteUserFromTeam;
using Getaway.Application.CQRS.Team.Commands.LeaveTeam;
using Getaway.Application.CQRS.Team.Commands.UpdateTeam;
using Getaway.Application.CQRS.Team.Queries.GetTeams;
using Getaway.Application.CQRS.Team.Queries.GetUsersByTeam;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;


namespace Getaway.Presentation.Controllers
{

    [ApiController]
    [Route("{userId}/api/[controller]")]
    public class TeamController(IMediator mediator) : ControllerBase
    {



        [HttpGet("list")]
        public async Task<ActionResult> GetListTeams(int userId)
        {
            List<TeamModel> teams = new List<TeamModel>();
            var teamsReply = mediator.Send(new GetTeamsQuery() { UserId = userId }).Result;

            foreach (var team in teamsReply)
            {

                teams.Add(new TeamModel()
                {
                    TeamId = team.ID.Value,
                    TeamTag = team.Tag,
                    TeamName = team.Name,
                    TeamLeadName = mediator.Send(new GetUserByIdQuery { UserId =team.TeamLeadId.Value}).Result.FirstName,
                    UserRole = team.TeamLeadId == userId ? (int) UserRole.LEAD : (int)UserRole.EMPLOYEE,
                    Users = mediator.Send(new GetUsersByTeamQuery() { TeamId = team.ID.Value }).Result.Select(t => new UserModel()
                    {
                        Email = t.Email,
                        FirstName = t.FirstName,
                        SecondName = t.SecondName,
                        LastName = t.LastName,
                        PhoneNumber = t.PhoneNumber,
                        UserTag = t.Tag
                    }).ToList()
                });

            }

            return Ok(teams);

        }



        [HttpPost("create")]
        public async Task<ActionResult> CreateTeam([FromBody] string name, int userId)
        {
            try
            {
                var team = mediator.Send(new CreateTeamCommand() { UserId = userId, Name = name }).Result;
                var user = mediator.Send(new GetUserByIdQuery() { UserId = userId}).Result;


                return Ok(new TeamModel()
                {
                    TeamId = team.ID.Value,
                    TeamName = team.Name,
                    UserRole = (int)UserRole.LEAD,
                    TeamTag = team.Tag,
                    Users = new List<UserModel> {new UserModel()
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        SecondName = user.SecondName,
                        LastName = user.LastName,
                        UserTag = user.Tag,
                        PhoneNumber = user.PhoneNumber
                    }}
                });

            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }




        [HttpPatch("{teamId}/update")]
        public async Task<ActionResult> Update(int teamId, [FromBody] TeamEntity teamEntity)
        {
            try
            {
                await mediator.Send(new UpdateTeamCommand() { LeadTag = teamEntity.TeamLeadTag, Name = teamEntity.Name, TeamId = teamId});

                return Ok();
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }


        [HttpPost("{teamId}/add-user")]
        public async Task<ActionResult> AddUser([FromBody] string userTag, int teamId)
        {
            try
            {
                await mediator.Send(new AddUserInTeamCommand() { TeamId = teamId, UserTag = userTag });
                return Ok();
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }


        [HttpDelete("{teamId}/delete-user/{userTag}")]
        public async Task<ActionResult> DeleteUser(string userTag, int teamId)
        {
            try
            {
                await mediator.Send(new DeleteUserFromTeamCommand() { TeamId = teamId, UserTag = userTag });
                return Ok();
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }



        [HttpDelete("{teamId}/leave")]
        public async Task<ActionResult> LeaveFromTeam(int userId, int teamId)
        {
            try
            {
                await mediator.Send(new LeaveTeamCommand() { TeamId = teamId, UserId = userId });
                return Ok();
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }

    }
}
