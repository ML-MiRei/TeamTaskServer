using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.UpdateProject
{
    public class UpdateProjectCommand : IRequest
    {
        public int ProjectId { get; set; }
        public string? Name { get; set; }
        public string? ProjectLeadTag { get; set; }
    }
}
