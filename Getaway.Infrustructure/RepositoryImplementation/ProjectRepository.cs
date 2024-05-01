using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.RepositoryImplementation
{
    public class ProjectRepository : IProjectRepository
    {
        public async Task AddTeamInProject(int projectId, string teamTag)
        {
            try
            {
                await Connections.ProjectServiceClient.AddTeamInProjectAsync(new AddTeamInProjectRequest() { ProjectId = projectId, TeamTag = teamTag });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task AddUserInProject(int projectId, string userTag)
        {
            try
            {
                await Connections.ProjectServiceClient.AddUserInProjectAsync(new AddUserInProjectRequest() { ProjectId = projectId, UserTag = userTag });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<ProjectEntity> CreateProjects(int userId, string name)
        {
            try
            {
                var project = await Connections.ProjectServiceClient.CreateProjectAsync(
                    new CreateProjectRequest()
                    {
                        UserId = userId,
                        Name = name
                    });
                return new ProjectEntity()
                {
                    ID = project.ProjectId,
                    ProjectName = name,
                    ProjectLeadId = project.ProjectLeadId
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task DeleteProjects(int projectId)
        {
            try
            {
                await Connections.ProjectServiceClient.DeleteProjectAsync(new DeleteProjectRequest() { ProjectId = projectId });
            }
            catch
            {
                throw new Exception();
            }
        }


        public async Task DeleteUserFromProject(int projectId, string userTag)
        {
            try
            {
                await Connections.ProjectServiceClient.DeleteUserFromProjectAsync(new DeleteUserFromProjectRequest() { ProjectId = projectId, UserTag = userTag });

            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task LeaveFromProject(int projectId, int userId)
        {
            try
            {
                await Connections.ProjectServiceClient.LeaveFromProjectAsync(new LeaveFromProjectRequest() { ProjectId = projectId, UserId = userId });

            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<ProjectEntity>> GetListProjects(int userId)
        {
            try
            {
                var projects = (await Connections.ProjectServiceClient.GetListProjectsAsync(new GetListProjectsRequest() { UserId = userId })).Projects;
                return projects.Select(p => new ProjectEntity()
                {
                    ProjectName = p.Name,
                    ID = p.ProjectId,
                    ProjectLeadId = p.ProjectLeadId
                }).ToList();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<UserEntity>> GetUsersByProject(int projectId)
        {
            try
            {
                var users = (await Connections.ProjectServiceClient.GetUsersFromProjectAsync(new GetUsersFromProjectRequest() { ProjectId = projectId })).Users;
                return users.Select(p => new UserEntity()
                {
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    SecondName = p.SecondName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    Tag = p.UserTag,
                    ID = p.UserId.Value

                }).ToList();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task UpdateProjects(int projectId, string? creatorTag, string? name)
        {
            try
            {
                Console.WriteLine("upd");
                await Connections.ProjectServiceClient.UpdateProjectAsync(new UpdateProjectRequest() { ProjectId = projectId, ProjectLeadTag = creatorTag, Name = name });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception();
            }
        }

        public async Task<ProjectEntity> GetProject(int projectId)
        {
            try
            {
                var project = await Connections.ProjectServiceClient.GetProjectAsync(new GetProjectRequest() { ProjectId = projectId});
                return new ProjectEntity()
                {
                    ProjectName = project.Name,
                    ID = project.ProjectId,
                    ProjectLeadId = project.ProjectLeadId
                };
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
