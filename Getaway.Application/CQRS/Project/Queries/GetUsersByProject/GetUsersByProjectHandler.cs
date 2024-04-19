using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using Getaway.Core.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Queries.GetUsersByProject
{
    public class GetUsersByProjectHandler(IProjectRepository projectRepository) : IRequestHandler<GetUsersByProjectQuery, List<UserEntity>>
    {
        public Task<List<UserEntity>> Handle(GetUsersByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = projectRepository.GetUsersByProject(request.ProjectId);
                return users;
            }
            catch
            {
                throw new NotFoundException();
            }
        }
    }
}
