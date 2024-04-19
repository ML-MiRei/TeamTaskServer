using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class Project_User
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
