using Getaway.Application.ServicesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Team.Commands.LeaveTeam
{
    public class LeaveTeamHandler(ITeamRepository teamRepository) : IRequestHandler<LeaveTeamCommand>
    {
        public async Task Handle(LeaveTeamCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await teamRepository.LeaveTeam(request.UserId, request.TeamId);
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
