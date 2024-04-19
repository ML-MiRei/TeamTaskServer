using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class Project
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public int ProjectLeadId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
    }
}
