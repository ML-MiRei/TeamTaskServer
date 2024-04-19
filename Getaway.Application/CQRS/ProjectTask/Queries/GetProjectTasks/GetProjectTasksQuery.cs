using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Queries.GetProjectTasks
{
    public class GetProjectTasksQuery : IRequest<List<ProjectTaskEntity>>
    {
        public int ProjectId {  get; set; }
    }
}
