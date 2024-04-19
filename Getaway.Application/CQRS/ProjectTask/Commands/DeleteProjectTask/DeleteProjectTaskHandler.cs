using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.DeleteProjectTask
{

    public class DeleteProjectTaskHandler(IProjectTaskRepository projectTaskRepository) : IRequestHandler<DeleteProjectTaskCommand>
    {
        public Task Handle(DeleteProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectTaskRepository.DeleteProjectTask(request.ProjectTaskId);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
