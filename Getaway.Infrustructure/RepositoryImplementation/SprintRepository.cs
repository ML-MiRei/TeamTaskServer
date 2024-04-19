using Getaway.Application.RepositoriesInterfaces;
using Getaway.Core.Entities;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Infrustructure.RepositoryImplementation
{
    public class SprintRepository : ISprintRepository
    {
        public async Task ChangeDateEndSprint(int sprintId, DateTime dateEnd)
        {
            try
            {
                await Connections.SprintServiceClient.ChangeDateEndSprintAsync(new ChangeDateEndSprintRequest() { DateEnd = Timestamp.FromDateTimeOffset(dateEnd), SprintId = sprintId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task ChangeDateStartSprint(int sprintId, DateTime dateStart)
        {
            try
            {
                await Connections.SprintServiceClient.ChangeDateStartSprintAsync(new ChangeDateStartSprintRequest() { DateStart = Timestamp.FromDateTimeOffset(dateStart), SprintId = sprintId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<SprintEntity> CreateSprint(int projectId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var sprint = await Connections.SprintServiceClient.CreateSprintAsync(new CreateSprintRequest()
                {
                    ProjectId = projectId,
                    DateEnd = Timestamp.FromDateTimeOffset(dateEnd),
                    DateStart = Timestamp.FromDateTimeOffset(dateStart)
                });
                return new SprintEntity
                {
                    DateStart = sprint.DateStart.ToDateTime(),
                    DateEnd = sprint.DateEnd.ToDateTime(),
                    ID = sprint.SprintId,
                    ProjectId = sprint.ProjectId,
                };
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task DeleteSprint(int sprintId)
        {
            try
            {
                await Connections.SprintServiceClient.DeleteSprintAsync(new DeleteSprintRequest() { SprintId = sprintId });
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<SprintEntity>> GetListSprints(int projectId)
        {
            try
            {
                var sprints = await Connections.SprintServiceClient.GetListSprintsAsync(new GetListSprintsRequest() { ProjectId = projectId });
                return sprints.Sprints.Select(s => new SprintEntity
                {
                    DateStart = s.DateStart.ToDateTime(),
                    DateEnd = s.DateEnd.ToDateTime(),
                    ID = s.SprintId,
                    ProjectId = s.ProjectId
                }).ToList();

            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
