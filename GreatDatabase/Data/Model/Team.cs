using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class Team
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TeamTag { get; set; }
        public string TeamName { get; set; }
        public int TeamLeadId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }

    }
}
