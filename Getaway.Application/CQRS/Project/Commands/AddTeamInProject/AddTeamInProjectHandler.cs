using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.AddTeamInProject
{
    public class AddTeamInProjectHandler(IProjectRepository projectRepository) : IRequestHandler<AddTeamInProjectCommand>
    {
        public async Task Handle(AddTeamInProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await projectRepository.AddTeamInProject(request.ProjectId, request.TeamTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
