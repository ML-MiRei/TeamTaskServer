using Azure;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server;
using Server.Data;
using Server.Data.Model;
using Services;


namespace Server.Services
{
    public class ProjectTaskApiService : ProjectTaskService.ProjectTaskServiceBase
    {
        MyDbContext db;
        public ProjectTaskApiService()
        {
            db = Config.myDbContext;
        }

        public override async Task<ProjectTaskModel> CreateProjectTasks(CreateProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                ProjectTask task = new ProjectTask()
                {
                    IsReady = request.Ready,
                    DateEnd = request.DateEnd.ToDateTime(),
                    DateStart = request.DateStart.ToDateTime(),
                    Name = request.Name,
                    Describe = request.Describe,
                    ID_Parent = request.IdParent,
                    ID_Project = request.IdProject
                };

                await db.ProjectTasks.AddAsync(task);

                db.SaveChanges();

                Console.WriteLine($"project task is created {task.Name}");


                return await Task.FromResult(new ProjectTaskModel()
                {
                    Ready = request.Ready,
                    DateEnd = request.DateEnd,
                    DateStart = request.DateStart,
                    Name = request.Name,
                    Describe = request.Describe,
                    IdParent = request.IdParent,
                    IdProject = request.IdProject,
                    IdProjectTasks = task.ID
                });


            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Add database error"));
            }

        }


        public override Task<ProjectTaskModel> DeleteProjectTasks(DeleteProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                ProjectTask task = db.ProjectTasks.First(u => u.ID == request.IdProjectTasks);
                db.ProjectTasks.Remove(task);

                db.SaveChanges();
                return Task.FromResult(new ProjectTaskModel()
                {
                    Ready = task.IsReady,
                    DateEnd = Timestamp.FromDateTime(task.DateEnd),
                    DateStart = Timestamp.FromDateTime(task.DateStart),
                    Name = task.Name,
                    Describe = task.Describe,
                    IdParent = task.ID_Parent,
                    IdProject = task.ID_Project,
                    IdProjectTasks = task.ID
                });
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }


        public override Task<ListProjectTasksReply> GetListProjectTasks(GetListProjectTasksRequest request, ServerCallContext context)
        {
            ListProjectTasksReply listTasks = new ListProjectTasksReply();
            List<ProjectTaskModel> replyList = (from p in db.ProjectTasks
                                                join pt in db.Projects_Teams on p.ID_Project equals pt.ID_Project
                                                join t in db.Teams_Users on pt.ID_Team equals t.ID_Team
                                                where t.ID_User == request.IdUser
                                                select new ProjectTaskModel()
                                                {
                                                    DateEnd = Timestamp.FromDateTimeOffset(p.DateEnd),
                                                    DateStart = Timestamp.FromDateTimeOffset(p.DateStart),
                                                    Describe = p.Describe,
                                                    IdParent = p.ID_Parent,
                                                    IdProject = p.ID_Project,
                                                    Name = p.Name,
                                                    Ready = p.IsReady,
                                                    IdProjectTasks = p.ID
                                                }).ToList();

            //List<ProjectTaskModel> replyList = db.ProjectTasks.Select(p => new ProjectTaskModel()
            //{
            //    DateEnd = Timestamp.FromDateTimeOffset(p.DateEnd),
            //    DateStart = Timestamp.FromDateTimeOffset(p.DateStart),
            //    Describe = p.Describe,
            //    IdParent = p.ID_Parent,
            //    IdProject = p.ID_Project,
            //    Name = p.Name,
            //    Ready = p.IsReady,
            //    IdProjectTasks = p.ID
            //}).ToList();
            listTasks.ProjectTasks.AddRange(replyList);
            return Task.FromResult(listTasks);
        }


        public override Task<ProjectTaskModel> UpdateProjectTasks(ProjectTaskModel request, ServerCallContext context)
        {
            try
            {
                ProjectTask projectTask = db.ProjectTasks.First(u => u.ID == request.IdProject);
                projectTask.Name = request.Name;
                projectTask.ID_Parent = request.IdParent;
                projectTask.DateStart = request.DateStart.ToDateTime();
                projectTask.DateEnd = request.DateEnd.ToDateTime();
                projectTask.IsReady = request.Ready;
                projectTask.Describe = request.Describe;
                projectTask.Name = request.Name;
                db.ProjectTasks.Update(projectTask);

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
