using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask
{
    public class SetExecutorProjectTaskCommand : IRequest
    {

        public int ProjectTaskId {  get; set; }
        public string UserTag { get; set; }

    }
}
