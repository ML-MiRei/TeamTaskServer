using Azure;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server;
using Server.Data;
using Server.Data.Model;
using Services;


namespace Server.Services
{
    public class ProjectApiService : ProjectService.ProjectServiceBase
    {
        MyDbContext db;
        public ProjectApiService()
        {
            db = Config.myDbContext;
        }


        public override async Task<ProjectModel> CreateProjects(CreateProjectRequest request, ServerCallContext context)
        {
            try
            {
                Project project = new Project()
                {
                    Name = request.Name,
                    ID_Creator = request.IdUser
                };

                await db.Projects.AddAsync(project);
                db.SaveChanges();
                await db.Projects_Teams.AddAsync(new Project_Team() { ID_Project = project.ID, ID_Team = request.IdTeam });
                db.SaveChanges();

                Console.WriteLine($"project is created {project.Name}");


                return await Task.FromResult(new ProjectModel()
                {
                    Name = request.Name,
                    IdCreator = request.IdUser,
                    IdProject = project.ID

                });


            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Add database error"));
            }
        }

        public override Task<ProjectModel> DeleteProjects(DeleteProjectRequest request, ServerCallContext context)
        {
            try
            {
                Project project = db.Projects.First(u => u.ID == request.IdProject && u.ID_Creator == request.IdUser);
                db.Projects.Remove(project);

                db.SaveChanges();
                return Task.FromResult(new ProjectModel()
                {
                    IdProject = project.ID,
                    IdCreator= request.IdUser,
                    Name = project.Name
                });
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }

        public override Task<ListProjectsReply> GetListProjects(GetListProjectsRequest request, ServerCallContext context)
        {
            ListProjectsReply listProjects = new ListProjectsReply();
            List<ProjectModel> replyList = (from p in db.Projects
                                                join pt in db.Projects_Teams on p.ID equals pt.ID_Project
                                                join t in db.Teams_Users on pt.ID_Team equals t.ID_Team
                                                where t.ID_User == request.IdUser
                                                select new ProjectModel()
                                                {
                                                    IdProject = p.ID,
                                                    IdCreator= request.IdUser,
                                                    Name = p.Name
                                                }).ToList();
            Console.WriteLine(request.IdUser);
            Console.WriteLine(replyList.Count);
            listProjects.Projects.AddRange(replyList);
            return Task.FromResult(listProjects);
        }

        public override Task<ProjectModel> UpdateProjects(ProjectModel request, ServerCallContext context)
        {
            try
            {
                Project project = db.Projects.First(u => u.ID == request.IdProject);
                project.Name = request.Name;
                project.ID_Creator = request.IdCreator;
                db.Projects.Update(project);

                db.SaveChanges();
                return Task.FromResult(request);
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }


     

    }

}
