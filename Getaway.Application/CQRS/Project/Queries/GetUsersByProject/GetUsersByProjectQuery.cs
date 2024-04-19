using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Queries.GetUsersByProject
{
    public class GetUsersByProjectQuery : IRequest<List<UserEntity>>
    {
        public int ProjectId { get; set; }
    }
}
