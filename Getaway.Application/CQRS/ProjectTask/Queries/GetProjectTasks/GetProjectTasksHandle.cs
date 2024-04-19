using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Queries.GetProjectTasks
{
    internal class GetProjectTasksHandle(IProjectTaskRepository projectTaskRepository) : IRequestHandler<GetProjectTasksQuery, List<ProjectTaskEntity>>
    {
        public Task<List<ProjectTaskEntity>> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectTasks = projectTaskRepository.GetListProjectTasks(request.ProjectId);
                return projectTasks;
            }
            catch
            {
                throw new NotFoundException();
            }
        }
    }
}
