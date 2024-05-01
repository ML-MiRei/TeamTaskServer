using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.AddUserInProject
{
    public class DeleteProjectHandler(IProjectRepository projectRepository) : IRequestHandler<AddUserInProjectCommand>
    {
        public async Task Handle(AddUserInProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await projectRepository.AddUserInProject(request.ProjectId, request.UserTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
