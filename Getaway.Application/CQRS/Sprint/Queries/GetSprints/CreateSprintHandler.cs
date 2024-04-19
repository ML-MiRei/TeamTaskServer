using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.GetSprints
{
    public class GetSprintHandler(ISprintRepository sprintRepository) : IRequestHandler<GetSprintsQuery, List<SprintEntity>>
    {
        public async Task<List<SprintEntity>> Handle(GetSprintsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sprint = await sprintRepository.GetListSprints(request.ProjectId);
                return sprint.Select(s => new SprintEntity
                {
                    DateStart = s.DateStart,
                    DateEnd = s.DateEnd,
                    ID = s.ID,
                    ProjectId = s.ProjectId,
                    ExecutorId = s.ExecutorId
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
