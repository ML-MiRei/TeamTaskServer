using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Model
{

    enum ChatTypeEnum
    {
        CHAT = 0,
        GROUP = 1
    }

    internal class Chat
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ID_Admin { get; set; }
        public int ChatType { get; set; }


    }
}
