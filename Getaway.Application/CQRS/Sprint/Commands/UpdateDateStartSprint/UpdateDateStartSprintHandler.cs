using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.UpdateDateStartSprint
{
    public class UpdateDateStartSprintHandler(ISprintRepository sprintRepository) : IRequestHandler<UpdateDateStartSprintCommand>
    {
        public async Task Handle(UpdateDateStartSprintCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await sprintRepository.ChangeDateStartSprint(request.SprintId, request.DateStart);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
