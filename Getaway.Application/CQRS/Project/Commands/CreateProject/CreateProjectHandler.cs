using Getaway.Application.CQRS.Project.Commands.CreateProject;
using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.CreateProject
{
    public class CreateProjectHandler(IProjectRepository projectRepository) : IRequestHandler<CreateProjectCommand, ProjectEntity>
    {
        public async Task<ProjectEntity> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await projectRepository.CreateProjects(request.UserId, request.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
