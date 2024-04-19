using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Getaway.Core.Common;

namespace Getaway.Core.Entities
{
    public class TeamEntity 
    {
        public int? ID { get; set; }
        public string? Tag { get; set; }
        public string? Name { get; set; }
        public string? TeamLeadTag { get; set; }
        public int? TeamLeadId { get; set; }

    }
}
