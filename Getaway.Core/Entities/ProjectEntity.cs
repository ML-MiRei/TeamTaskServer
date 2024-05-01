using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Getaway.Core.Common;

namespace Getaway.Core.Entities
{
    public class ProjectEntity : IBaseEntity
    {
        public int ID { get; set; }
        public int? ProjectLeadId { get; set; }
        public string? ProjectLeadTag { get; set; }
        public string? ProjectLeadName { get; set; }
        public string? ProjectName { get; set; }

    }
}
