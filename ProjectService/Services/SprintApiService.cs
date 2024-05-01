using Google.Protobuf.WellKnownTypes;
using GreatDatabase.Data;
using GreatDatabase.Data.Model;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace ProjectService.Services
{
    public class SprintApiService : SprintService.SprintServiceBase
    {
        private static MyDbContext db;
        private static ILogger<SprintApiService> _logger;

        public SprintApiService(ILogger<SprintApiService> logger)
        {
            _logger = logger;
            db = new MyDbContext();
        }

        public override Task<SprintReply> GetSprint(GetSprintRequest request, ServerCallContext context)
        {
            var sprint = db.Sprints.First(s => s.ID == request.SprintId);
            return Task.FromResult(new SprintReply
            {
                DateEnd = Timestamp.FromDateTimeOffset(sprint.DateEnd),
                SprintId = sprint.ID,
                ProjectId = sprint.ProjectId,
                DateStart = Timestamp.FromDateTimeOffset(sprint.DateStart)
            });
        }

        public async override Task<VoidSprintReply> ChangeDateEndSprint(ChangeDateEndSprintRequest request, ServerCallContext context)
        {
            try
            {
                Sprint sprint = db.Sprints.First(s => s.ID == request.SprintId);
                sprint.DateEnd = request.DateEnd.ToDateTime().AddDays(1);

                db.Sprints.Update(sprint);
                await db.SaveChangesAsync();


                _logger.LogInformation($"Date end of sprint with id = {request.SprintId} is changed");

                return new VoidSprintReply();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Update db error"));
            }
        }

        public async override Task<VoidSprintReply> ChangeDateStartSprint(ChangeDateStartSprintRequest request, ServerCallContext context)
        {
            try
            {

                Sprint sprint = db.Sprints.First(s => s.ID == request.SprintId);
                sprint.DateStart = request.DateStart.ToDateTime().AddDays(1);

                db.Sprints.Update(sprint);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Date start of sprint with id = {request.SprintId} is changed");


                return new VoidSprintReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Update db error"));
            }
        }

        public async override Task<CreateSprintReply> CreateSprint(CreateSprintRequest request, ServerCallContext context)
        {
            try
            {

                Sprint sprint = new Sprint()
                {
                    DateStart = request.DateStart.ToDateTime(),
                    DateEnd = request.DateEnd.ToDateTime(),
                    ProjectId = request.ProjectId,
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                };

                await db.AddAsync(sprint);
                await db.SaveChangesAsync();

                _logger.LogInformation($"Sprint with id = {sprint.ID} is created");


                return new CreateSprintReply
                {
                    DateEnd = Timestamp.FromDateTimeOffset(sprint.DateEnd),
                    DateStart = Timestamp.FromDateTimeOffset(sprint.DateStart),
                    ProjectId = sprint.ProjectId,
                    SprintId = sprint.ID
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Create db error"));
            }
        }

        public async override Task<VoidSprintReply> DeleteSprint(DeleteSprintRequest request, ServerCallContext context)
        {
            try
            {

                Sprint sprint = db.Sprints.First(s => s.ID == request.SprintId);

                db.Sprints.Remove(sprint);

                foreach (var item in db.ProjectTasks.Where(t => t.SprintId == request.SprintId))
                {
                    item.Status = 0;
                    db.ProjectTasks.Update(item);
                }

                await db.SaveChangesAsync();

                _logger.LogInformation($"Sprint with id = {request.SprintId} is deleted");

                return new VoidSprintReply();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Delete db error"));
            }
        }

        public async override Task<GetListSprintsReply> GetListSprints(GetListSprintsRequest request, ServerCallContext context)
        {
            GetListSprintsReply listSprints = new GetListSprintsReply();
            List<SprintReply> sprints = db.Sprints.Where(s => s.ProjectId == request.ProjectId).Select(s => new SprintReply
            {
                DateEnd = Timestamp.FromDateTimeOffset(s.DateEnd),
                SprintId = s.ID,
                ProjectId = s.ProjectId,
                DateStart = Timestamp.FromDateTimeOffset(s.DateStart)
            }).ToList();


            listSprints.Sprints.AddRange(sprints);

            _logger.LogInformation($"Return {sprints.Count} sprints");

            return listSprints;
        }
    }
}
