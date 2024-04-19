using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.ReturnsModels
{
    public class SprintModel
    {
        public int SprintId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<ProjectTaskModel> Tasks { get; set; }

    }
}
