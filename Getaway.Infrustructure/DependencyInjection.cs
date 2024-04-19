using Getaway.Application.RepositoriesInterfaces;
using Getaway.Application.ServicesInterfaces;
using Getaway.Infrustructure.RepositoryImplementation;
using Getaway.Infrustructure.Services.Implementations;
using Getaway.Infrustructure.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrustructure(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<ITeamRepository, TeamRepository>()
                .AddSingleton<IAuthenticationService, AuthenticationServiceImpl>()
                .AddSingleton<IMessangerRepository, MessangerRepository>()
                .AddSingleton<IProjectTaskRepository, ProjectTaskRepository>()
                .AddSingleton<ISprintRepository, SprintRepository>()
                .AddSingleton<INotificationRepository, NotificationRepository>()
                .AddSingleton<IProjectRepository, ProjectRepository>();
            
            return services;
        }
    }
}
