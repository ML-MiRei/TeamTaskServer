using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.DeleteTeamFromProject
{
    public class DeleteTeamFromProjectCommand : IRequest
    {
        public int ProjectId { get; set; }
        public string TeamTag { get; set; }
    }
}
