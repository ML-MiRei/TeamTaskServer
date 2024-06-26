﻿using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.GetSprints
{
    public class GetSprintsQuery : IRequest<List<SprintEntity>>
    {
        public int ProjectId { get; set; }
    }
}
