using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask
{
    public class DeleteProjectTaskCommand : IRequest
    {

        public int ProjectTaskId {  get; set; }

    }
}
