using Getaway.Application.CQRS.Project.Commands.CreateProject;
using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.UpdateProject
{
    public class UpdateProjectHandler(IProjectRepository projectRepository) : IRequestHandler<UpdateProjectCommand>
    {
        public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await projectRepository.UpdateProjects(request.ProjectId, request.ProjectLeadTag, request.Name);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
