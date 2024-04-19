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
        public Task<ProjectEntity> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = projectRepository.CreateProjects(request.UserId, request.Name);
                return project;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
