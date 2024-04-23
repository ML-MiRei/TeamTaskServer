using Azure;
using Google.Protobuf.WellKnownTypes;
using GreatDatabase.Data;
using GreatDatabase.Data.Enums;
using GreatDatabase.Data.Model;
using Grpc.Core;



namespace ProjectService.Services
{
    public class ProjectTaskApiService : ProjectTaskService.ProjectTaskServiceBase
    {
        private static MyDbContext db;
        private static ILogger<ProjectTaskApiService> _logger;

        public ProjectTaskApiService(ILogger<ProjectTaskApiService> logger)
        {
            _logger = logger;                    
            db = new MyDbContext();
        }






        public async override Task<ProjectTaskReply> CreateProjectTask(CreateProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"sprint id = {request.SprintId}");


                ProjectTask task = new ProjectTask()
                {
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,

                    Title = request.Title,
                    Details = request.Details,
                    ProjectId = request.ProjectId,
                    SprintId= request.SprintId,
                    Status = request.Status
                };

                await db.ProjectTasks.AddAsync(task);
                db.SaveChanges();


                Console.WriteLine("status  = " + request.Status);

                _logger.LogInformation($"Project task '{task.Title}' is created");


                return await Task.FromResult(new ProjectTaskReply()
                {
                    Title = task.Title,
                    Details = task.Details,
                    SprintId = task.SprintId,
                    ProjectTasksId = task.ID,
                    Status = task.Status,
                    ProjectId = task.ProjectId
                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Add database error"));
            }
        }

        public async override Task<VoidProjectTaskReply> AddProjectTaskInSprint(AddProjectTaskInSprintRequest request, ServerCallContext context)
        {
            try
            {

                ProjectTask projectTask = db.ProjectTasks.First(u => u.ID == request.ProjectTasksId);

                projectTask.SprintId = request.SprintId;
                projectTask.Status = (int)ProjectTaskStatusEnum.TODO;

                db.ProjectTasks.Update(projectTask);

                db.SaveChanges();

                _logger.LogInformation($"ProjectTask with id = {request.ProjectTasksId} add in sprint id = {request.SprintId}");


                return new VoidProjectTaskReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException
                    .Message);
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }


        public async override Task<VoidProjectTaskReply> DeleteProjectTask(DeleteProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                ProjectTask task = db.ProjectTasks.First(u => u.ID == request.ProjectTaskId);
                db.ProjectTasks.Remove(task);

                await db.SaveChangesAsync();


                _logger.LogInformation($"ProjectTask with id = {request.ProjectTaskId} is deleted");

                return new VoidProjectTaskReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "delete database error"));
            }
        }


        public override Task<ListProjectTasksReply> GetListProjectTasks(GetListProjectTasksRequest request, ServerCallContext context)
        {
            ListProjectTasksReply listTasks = new ListProjectTasksReply();
            List<ProjectTaskReply> replyList = (from p in db.ProjectTasks
                                                where p.ProjectId == request.ProjectId
                                                select new ProjectTaskReply()
                                                {
                                                 
                                                    SprintId = p.SprintId,
                                                    Details = p.Details,
                                                    Title = p.Title,
                                                    ProjectTasksId = p.ID,
                                                    UserId = p.UserId,
                                                    Status = p.Status,
                                                    ProjectId = p.ProjectId
                                                       
                                                }).ToList();

            listTasks.ProjectTasks.AddRange(replyList);

            _logger.LogInformation($"Return {replyList.Count} project tasks");

            return Task.FromResult(listTasks);
        }


        public async override Task<VoidProjectTaskReply> UpdateProjectTask(UpdateProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                ProjectTask projectTask = db.ProjectTasks.First(u => u.ID == request.ProjectTasksId);

                projectTask.Title = String.IsNullOrEmpty(request.Title) ? projectTask.Title : request.Title;
                projectTask.Details = String.IsNullOrEmpty(request.Details) ? projectTask.Details : request.Details;
     
                db.ProjectTasks.Update(projectTask);

                await db.SaveChangesAsync();

                _logger.LogInformation($"ProjectTask with id = {request.ProjectTasksId} is updated");


                return new VoidProjectTaskReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }

       

        public async override Task<VoidProjectTaskReply> ChangeStatusProjectTask(ChangeStatusProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                ProjectTask projectTask = db.ProjectTasks.First(u => u.ID == request.ProjectTasksId);

                projectTask.Status = request.Status;

                db.ProjectTasks.Update(projectTask);

                await db.SaveChangesAsync();

                _logger.LogInformation($"For projectTask with id = {request.ProjectTasksId} change status on {(ProjectTaskStatusEnum)request.Status}");


                return new VoidProjectTaskReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }

        public async override Task<VoidProjectTaskReply> SetExecutorProjectTask(SetExecutorProjectTaskRequest request, ServerCallContext context)
        {
            try
            {
                ProjectTask projectTask = db.ProjectTasks.First(u => u.ID == request.ProjectTasksId);

                projectTask.UserId = db.Users.First(u => u.UserTag == request.UserTag).ID;

                db.ProjectTasks.Update(projectTask);

                await db.SaveChangesAsync();

                _logger.LogInformation($"For projectTask with id = {request.ProjectTasksId} set executor as user with tag = {request.UserTag}");


                return new VoidProjectTaskReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "update database error"));
            }
        }


    }
}
