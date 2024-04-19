using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class Message
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TextMessage { get; set; }
        public DateTime DateCreated { get; set; }
        public int ChatId { get; set; }
        public int CreatorId { get; set; }

        public DateTime LastModified { get; set; }
    }
}
