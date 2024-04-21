
using Google.Protobuf.WellKnownTypes;
using GreatDatabase.Data;
using GreatDatabase.Data.Model;
using Grpc.Core;


namespace ProjectService.Services
{
    public class ProjectApiService : ProjectService.ProjectServiceBase
    {
        private static MyDbContext db;
        private static ILogger<ProjectApiService> _logger;


        public ProjectApiService(ILogger<ProjectApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }


        public async override Task<ProjectModel> GetProject(GetProjectRequest request, ServerCallContext context)
        {
            var project = await db.Projects.FindAsync(request.ProjectId);
            return new ProjectModel
            {
                Name = project.ProjectName,
                ProjectId = project.ID,
                ProjectLeadId = project.ProjectLeadId
            };
        }

        public async override Task<VoidProjectReply> LeaveFromProject(LeaveFromProjectRequest request, ServerCallContext context)
        {
            try
            {
                Project_User project_user = db.Projects_Users.First(p => p.UserId == request.UserId && p.ProjectId == request.ProjectId);

                db.Projects_Users.Remove(project_user);
                await db.SaveChangesAsync();

                if (!db.Projects.Any(p => p.ID == request.ProjectId))
                {
                    Project project = db.Projects.Find(request.ProjectId);
                    db.Projects.Remove(project);
                    await db.SaveChangesAsync();

                }


                _logger.LogInformation($"User {request.UserId} delete from project {request.ProjectId}");


                return new VoidProjectReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }

        public override async Task<ProjectModel> CreateProject(CreateProjectRequest request, ServerCallContext context)
        {
            try
            {
                Project project = new Project()
                {
                    ProjectName = request.Name,
                    ProjectLeadId = request.UserId,
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now
                };

                await db.Projects.AddAsync(project);
                db.SaveChanges();


                _logger.LogInformation($"Project is created {project.ProjectName}");


                await db.Projects_Users.AddAsync(new Project_User()
                {
                    DateCreated = DateTime.Now,
                    ProjectId = project.ID,
                    UserId = request.UserId
                });
                db.SaveChanges();

                _logger.LogInformation($"User {request.UserId} add in project {project.ProjectName}");

                return new ProjectModel()
                {
                    Name = request.Name,
                    ProjectLeadId = project.ProjectLeadId,
                    ProjectId = project.ID

                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Add database error"));
            }
        }


        public override Task<VoidProjectReply> DeleteProject(DeleteProjectRequest request, ServerCallContext context)
        {
            try
            {
                Project project = db.Projects.First(u => u.ID == request.ProjectId);
                db.Projects.Remove(project);

                db.SaveChanges();

                _logger.LogInformation($"Deleted project {request.ProjectId}");

                return Task.FromResult(new VoidProjectReply());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }



        public override Task<ListProjectsReply> GetListProjects(GetListProjectsRequest request, ServerCallContext context)
        {
            ListProjectsReply listProjects = new ListProjectsReply();
            List<ProjectModel> replyList = (from p in db.Projects
                                            join pt in db.Projects_Users on p.ID equals pt.ProjectId
                                            where pt.UserId == request.UserId
                                            select new ProjectModel()
                                            {
                                                ProjectId = p.ID,
                                                ProjectLeadId = p.ProjectLeadId,
                                                Name = p.ProjectName
                                            }).ToList();


            listProjects.Projects.AddRange(replyList.Distinct());

            _logger.LogInformation($"Return {replyList.Count} projects");

            return Task.FromResult(listProjects);
        }

        public override Task<VoidProjectReply> UpdateProject(UpdateProjectRequest request, ServerCallContext context)
        {
            try
            {
                Project project = db.Projects.First(u => u.ID == request.ProjectId);

                project.ProjectName = String.IsNullOrEmpty(request.Name) ? project.ProjectName : request.Name;
                project.ProjectLeadId = String.IsNullOrEmpty(request.ProjectLeadTag) ? project.ProjectLeadId : db.Users.First(u => u.UserTag == request.ProjectLeadTag).ID;
                db.Projects.Update(project);

                db.SaveChanges();

                _logger.LogInformation($"Updated project: {project.ProjectName}");

                return Task.FromResult(new VoidProjectReply());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }



        public override async Task<VoidProjectReply> AddTeamInProject(AddTeamInProjectRequest request, ServerCallContext context)
        {
            try
            {
                var users = db.Teams_Users.Where(tu => tu.TeamId == db.Teams.First(t => t.TeamTag == request.TeamTag).ID).Select(u => u.UserId);

                foreach (var user in users)
                {
                    await db.Projects_Users.AddAsync(new Project_User()
                    {
                        DateCreated = DateTime.Now,
                        ProjectId = request.ProjectId,
                        UserId = user
                    });

                }


                db.SaveChanges();

                _logger.LogInformation($"Users from team {request.TeamTag} add in project {request.ProjectId}");

                return new VoidProjectReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Add database error"));
            }
        }


        public override Task<GetUsersFromProjectReply> GetUsersFromProject(GetUsersFromProjectRequest request, ServerCallContext context)
        {
            GetUsersFromProjectReply listUsers = new GetUsersFromProjectReply();
            List<UserFromProjectReply> replyList = (from pt in db.Projects_Users
                                                    join u in db.Users on pt.UserId equals u.ID
                                                    where pt.ProjectId == request.ProjectId
                                                    select new UserFromProjectReply()
                                                    {
                                                        UserTag = u.UserTag,
                                                        FirstName = u.FirstName,
                                                        SecondName = u.SecondName,
                                                        LastName = u.LastName,
                                                        PhoneNumber = u.PhoneNumber,
                                                        Email = u.Email

                                                    }).ToList();


            listUsers.Users.AddRange(replyList.Distinct());

            _logger.LogInformation($"Return {replyList.Count} users");

            return Task.FromResult(listUsers);
        }


        public override async Task<VoidProjectReply> AddUserInProject(AddUserInProjectRequest request, ServerCallContext context)
        {
            try
            {
                await db.Projects_Users.AddAsync(new Project_User()
                {
                    DateCreated = DateTime.Now,
                    ProjectId = request.ProjectId,
                    UserId = db.Users.First(t => t.UserTag == request.UserTag).ID,
                });
                db.SaveChanges();

                _logger.LogInformation($"User {request.UserTag} add in project {request.ProjectId}");

                return new VoidProjectReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Add database error"));
            }
        }


        public override Task<VoidProjectReply> DeleteUserFromProject(DeleteUserFromProjectRequest request, ServerCallContext context)
        {
            try
            {
                int userId = db.Users.First(t => t.UserTag == request.UserTag).ID;
                Project_User project_user = db.Projects_Users.First(p => p.UserId == userId && p.ProjectId == request.ProjectId);

                db.Projects_Users.Remove(project_user);
                db.SaveChanges();


                _logger.LogInformation($"User {request.UserTag} delete from project {request.ProjectId}");


                return Task.FromResult(new VoidProjectReply());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }
    }

}
