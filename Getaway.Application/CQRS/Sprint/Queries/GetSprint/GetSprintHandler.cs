using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.GetSprint
{
    public class GetSprintHandler(ISprintRepository sprintRepository) : IRequestHandler<GetSprintQuery, SprintEntity>
    {
        public async Task<SprintEntity> Handle(GetSprintQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sprint = await sprintRepository.GetSprint(request.SprintId);
                return new SprintEntity
                {
                    DateStart = sprint.DateStart,
                    DateEnd = sprint.DateEnd,
                    ID = sprint.ID,
                    ProjectId = sprint.ProjectId,
                    ExecutorId = sprint.ExecutorId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
