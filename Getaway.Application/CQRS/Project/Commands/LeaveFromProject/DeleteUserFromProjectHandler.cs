using Getaway.Application.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Commands.LeaveFromProject
{
    public class LeaveFromProjectDeleteUserFromProjectHandler(IProjectRepository projectRepository) : IRequestHandler<LeaveFromProjectCommand>
    {
        public async Task Handle(LeaveFromProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await projectRepository.LeaveFromProject(request.ProjectId, request.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
