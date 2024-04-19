using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Model
{
    internal class Team
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ID_Lead { get; set; }
        public string Name { get; set; }



    }
}
