using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Getaway.Core.Common;

namespace Getaway.Core.Entities
{
    public class ProjectTaskEntity : IBaseEntity
    {
        public int ID { get; set; }
        public int? SprintId { get; set; }
        public int? ProjectId { get; set; }
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public int? Status { get; set; }
        public int? ExecutorId { get; set; }
        public string? ExecutorTag { get; set; }
        public string? ExecutorName { get; set; }

    }
}
