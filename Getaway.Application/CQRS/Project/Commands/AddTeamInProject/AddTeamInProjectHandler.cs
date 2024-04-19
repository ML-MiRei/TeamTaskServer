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
        public Task Handle(AddTeamInProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectRepository.AddTeamInProject(request.ProjectId, request.TeamTag);
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
