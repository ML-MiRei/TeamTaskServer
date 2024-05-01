using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.DeleteTeamFromProject
{
    public class DeleteUserFromProjectHandler(IProjectRepository projectRepository) : IRequestHandler<DeleteTeamFromProjectCommand>
    {
        public async Task Handle(DeleteTeamFromProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await projectRepository.DeleteUserFromProject(request.ProjectId, request.TeamTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
