using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;

namespace Getaway.Application.CQRS.Project.Queries.GetProject
{
    public class GetProjectHandler(IProjectRepository projectRepository) : IRequestHandler<GetProjectQuery, ProjectEntity>
    {
        public Task<ProjectEntity> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projects = projectRepository.GetProject(request.ProjectId);
                return projects;
            }
            catch
            {
                throw new NotFoundException();
            }
        }
    }
}
