using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.DeleteSprint
{
    public class DeleteSprintHandler(ISprintRepository sprintRepository) : IRequestHandler<DeleteSprintCommand>
    {
        public async Task Handle(DeleteSprintCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await sprintRepository.DeleteSprint(request.SprintId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
