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
        public Task Handle(DeleteTeamFromProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectRepository.DeleteUserFromProject(request.ProjectId, request.TeamTag);
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
