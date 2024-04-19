using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.LeaveFromProject
{
    public class LeaveFromProjectCommand : IRequest
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
    }
}
