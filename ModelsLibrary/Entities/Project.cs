using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Common;

namespace ModelsLibrary.Entities
{
    public class Project : IAuditableEntity
    {
        public int ID { get; set; }
        public string NameLeader { get; set; }
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
        public List<User> Users { get; set; }
        public List<ProjectTask> Tasks { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
