using ModelsLibrary.Common;
using ModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelsLibrary.Entities
{


    public class Chat : IAuditableEntity
    {
        public int ID { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }
        public int Type { get; set; }
        public string Image { get; set; }
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
