﻿using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.CQRS.Sprint.Commands.CreateSprint
{
    public class CreateSprintHandler(ISprintRepository sprintRepository) : IRequestHandler<CreateSprintCommand, SprintEntity>
    {
        public async Task<SprintEntity> Handle(CreateSprintCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await sprintRepository.CreateSprint(request.ProjectId, request.DateStart, request.DateEnd);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }
    }
}
