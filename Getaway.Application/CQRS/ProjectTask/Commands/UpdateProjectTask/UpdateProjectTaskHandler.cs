using Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask;
using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.UpdateProjectTask
{

    public class UpdateProjectTaskHandler(IProjectTaskRepository projectTaskRepository) : IRequestHandler<UpdateProjectTaskCommand>
    {
        public async Task Handle(UpdateProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await projectTaskRepository.UpdateProjectTask(request.ProjectTaskId, request.Title, request.Details);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
