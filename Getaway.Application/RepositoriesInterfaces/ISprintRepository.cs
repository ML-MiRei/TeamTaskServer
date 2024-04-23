using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.RepositoriesInterfaces
{
    public interface ISprintRepository
    {
        Task<SprintEntity> CreateSprint(int projectId, DateTime dateStart, DateTime dateEnd);
        Task DeleteSprint(int sprintId);
        Task ChangeDateStartSprint(int sprintId, DateTime dateStart);
        Task ChangeDateEndSprint(int sprintId, DateTime dateEnd);
        Task<List<SprintEntity>> GetListSprints(int projectId);
        Task<SprintEntity> GetSprint(int sprintId);
    }
}
