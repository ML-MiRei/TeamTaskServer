using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.UpdateDateEndSprint
{
    public class UpdateDateEndSprintCommand : IRequest
    {
        public int SprintId { get; set; }
        public DateTime DateEnd {  get; set; }

    }
}
