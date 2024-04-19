using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Model
{
    internal class Project_Team
    {
        public int ID_Project { get; set; }
        public int ID_Team { get; set; }


    }
}
