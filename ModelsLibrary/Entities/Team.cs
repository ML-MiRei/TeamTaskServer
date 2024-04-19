using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Common;

namespace ModelsLibrary.Entities
{
    public class Team : IAuditableEntity
    {
        public int ID { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string NameLead { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
