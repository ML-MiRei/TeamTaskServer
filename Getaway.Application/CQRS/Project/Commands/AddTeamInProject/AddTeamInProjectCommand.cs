using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.AddTeamInProject
{
    public class AddTeamInProjectCommand : IRequest
    {
        public int ProjectId { get; set; }
        public string TeamTag { get; set; }
    }
}
