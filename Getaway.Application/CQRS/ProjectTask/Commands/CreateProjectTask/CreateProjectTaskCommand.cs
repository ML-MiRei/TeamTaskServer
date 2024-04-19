using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask
{
    public class CreateProjectTaskCommand : IRequest<ProjectTaskEntity>
    {

        public int? SprintId { get; set; }
        public int ProjectId { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }

    }
}
