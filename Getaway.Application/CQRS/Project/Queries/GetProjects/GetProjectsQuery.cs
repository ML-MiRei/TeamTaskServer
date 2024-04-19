using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Project.Queries.GetProjects
{
    public class GetProjectsQuery : IRequest<List<ProjectEntity>>
    {
        public int UserId { get; set; }
    }
}
