using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.SetExecutorProjectTask
{

    public class SetExecutorProjectTaskHandler(IProjectTaskRepository projectTaskRepository) : IRequestHandler<SetExecutorProjectTaskCommand>
    {
        public Task Handle(SetExecutorProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectTaskRepository.SetExecutorProjectTask(request.ProjectTaskId, request.UserTag);
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
