using ApiGetaway.Logic.TeamLogic.Commands.CreateTeam;
using ApiGetaway.Logic.TeamLogic.Commands.LeaveTeam;
using ApiGetaway.Logic.TeamLogic.Queries.GetTeams;
using ApiGetaway.Logic.TeamLogic.Queries.GetUsersByTeam;
using ApiGetaway.Logic.UserLogic.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary;
using ModelsLibrary.Entities;
using TeamTaskServerAPI;

namespace ApiGetaway.Infrustructure.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TeamController(IMediator mediator) : ControllerBase
    {



        [HttpGet("list/user-tag={userTag}")]
        public async Task<ActionResult> GetListTeams(string userTag)
        {
            List<Team> teams = new List<Team>();
            var teamsReply = mediator.Send(new GetTeamsQuery(){ UserTag = userTag}).Result;
            foreach (var team in teamsReply)
            {

                teams.Add(new Team()
                {
                    ID = team.IdTeam,
                    Tag = team.TagTeam,
                    Name = team.Name,
                    NameLead = mediator.Send(new GetUserByTagQuery() { UserTag = userTag }).Result.FirstName
                }) ;
            }
            return Ok(teams);

        }

        [HttpDelete("delete-team/team_tag={teamTag}&user-tag={userTag}")]
        public async Task<ActionResult> Delete(string teamTag, string userTag)
        {
            try
            {
                await mediator.Send(new LeaveTeamCommand() { TeamTag = teamTag, UserTag = userTag });
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }


        [HttpPost("create/user-tag={userTag}&team-name={name}")]
        public async Task<ActionResult> CreateTeam(string userTag, string name)
        {
            try
            {


                var team = mediator.Send(new CreateTeamCommand() { UserTag = userTag, Name = name }).Result;

               return Ok(team);


                //return Ok(new Team()
                //{
                //    ID = team.IdTeam,
                //    Name = team.Name,
                //    NameLead = userReply.LastName,
                //    Members = new List<User> { user }
                //})
                //;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new EmptyResult();
            }
        }




        [HttpPatch("{team}")]
        public void Patch(Team team)
        {
            //...
        }


        [HttpPost("user/{userTag}")]
        public void AddUser(string userTag)
        {
            //...
        }


        [HttpDelete("user/{userTag}")]
        public void DeleteUser(string userTag)
        {
            //..
        }


    }
}
