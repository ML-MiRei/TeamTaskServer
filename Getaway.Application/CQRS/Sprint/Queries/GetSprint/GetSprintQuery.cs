using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.GetSprint
{
    public class GetSprintQuery : IRequest<SprintEntity>
    {
        public int SprintId { get; set; }
    }
}
