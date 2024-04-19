using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;

namespace Getaway.Application.CQRS.Project.Queries.GetProjects
{
    public class GetProjectsHandler(IProjectRepository projectRepository) : IRequestHandler<GetProjectsQuery, List<ProjectEntity>>
    {
        public Task<List<ProjectEntity>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projects = projectRepository.GetListProjects(request.UserId);
                return projects;
            }
            catch
            {
                throw new NotFoundException();
            }
        }
    }
}
