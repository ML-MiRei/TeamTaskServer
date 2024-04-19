using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.DeleteUserFromProject
{
    public class DeleteUserFromProjectHandler(IProjectRepository projectRepository) : IRequestHandler<DeleteUserFromProjectCommand>
    {
        public Task Handle(DeleteUserFromProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                projectRepository.DeleteUserFromProject(request.ProjectId, request.UserTag);
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
