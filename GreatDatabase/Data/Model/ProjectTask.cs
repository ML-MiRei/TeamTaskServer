using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class ProjectTask
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Details { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public int? SprintId { get; set; }
        public int ProjectId { get; set; }
        public int? UserId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }

    }
}
