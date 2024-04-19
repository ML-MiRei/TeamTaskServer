using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.UpdateDateEndSprint
{
    public class UpdateDateEndSprintHandler(ISprintRepository sprintRepository) : IRequestHandler<UpdateDateEndSprintCommand>
    {
        public async Task Handle(UpdateDateEndSprintCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await sprintRepository.ChangeDateEndSprint(request.SprintId, request.DateEnd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
