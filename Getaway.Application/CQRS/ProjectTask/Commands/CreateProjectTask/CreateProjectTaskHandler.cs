using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.CreateProjectTask
{
    public class CreateProjectTaskHandler(IProjectTaskRepository projectTaskRepository) : IRequestHandler<CreateProjectTaskCommand, ProjectTaskEntity>
    {
        public async Task<ProjectTaskEntity> Handle(CreateProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await projectTaskRepository.CreateProjectTask(request.ProjectId, request.SprintId, request.Title, request.Details, request.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
