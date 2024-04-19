using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Team.Commands.LeaveTeam
{
    public class LeaveTeamCommand : IRequest
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
    }
}
