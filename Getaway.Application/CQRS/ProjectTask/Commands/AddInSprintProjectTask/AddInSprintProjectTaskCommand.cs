using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.AddInSprintProjectTask
{
    public class AddInSprintProjectTaskCommand : IRequest
    {

        public int ProjectTaskId {  get; set; }
        public int SprintId { get; set; }

    }
}
