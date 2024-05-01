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
        public async Task Handle(ChangeStatusProjectTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await projectTaskRepository.ChangeStatusProjectTask(request.ProjectTaskId, request.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
