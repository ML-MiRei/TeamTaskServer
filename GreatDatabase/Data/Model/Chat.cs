using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{



    public class Chat
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ChatName { get; set; }
        public int? AdminId { get; set; }
        public int Type { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }


    }
}
