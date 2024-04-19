using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask
{
    public class UpdateProjectTaskCommand : IRequest
    {

        public int ProjectTaskId {  get; set; }
        public string Title { get; set; }
        public string Details { get; set; }

    }
}
