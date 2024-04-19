using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Common;

namespace ModelsLibrary.Entities
{
    public class ProjectTask : IAuditableEntity
    {
        public int ID { get; set; }
        public int? ProjectId { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public bool Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<ProjectTask> ChildTasks { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
