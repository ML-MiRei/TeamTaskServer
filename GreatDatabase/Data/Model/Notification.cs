using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Details { get; set; }
        public string Title { get; set; }

    }
}
