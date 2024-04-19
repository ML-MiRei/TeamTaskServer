using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.ChangeStatusProjectTask
{

    public class DeleteProjectTaskHandler(IProjectTaskRepository projectTaskRepository) : IRequestHandler<ChangeStatusProjectTaskCommand>
    {
        public Task Handle(ChangeStatusProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectTaskRepository.ChangeStatusProjectTask(request.ProjectTaskId, request.Status);
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
