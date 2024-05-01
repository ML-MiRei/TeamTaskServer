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
        public async Task Handle(AddInSprintProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await projectTaskRepository.AddInSprintProjectTask(request.ProjectTaskId, request.SprintId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
