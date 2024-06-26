﻿
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
        Random random = new Random();


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
                    TeamLeadName = mediator.Send(new GetUserByIdQuery { UserId = team.TeamLeadId.Value }).Result.FirstName,
                    UserRole = team.TeamLeadId == userId ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE,
                    Users = mediator.Send(new GetUsersByTeamQuery() { TeamId = team.ID.Value }).Result.Select(t => new UserModel()
                    {
                        Email = t.Email,
                        FirstName = t.FirstName,
                        SecondName = t.SecondName,
                        LastName = t.LastName,
                        PhoneNumber = t.PhoneNumber,
                        UserTag = t.Tag,
                        ColorNumber = random.Next(5)
                    }).ToList()
                }) ;

            }

            return Ok(teams);

        }

    }
}
