using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.ProjectTask.Commands.AddInSprintProjectTask
{

    public class AddInSprintProjectTaskHandler(IProjectTaskRepository projectTaskRepository) : IRequestHandler<AddInSprintProjectTaskCommand>
    {
        public Task Handle(AddInSprintProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectTaskRepository.AddInSprintProjectTask(request.ProjectTaskId, request.SprintId);
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
