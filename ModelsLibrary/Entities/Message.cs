using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Common;

namespace ModelsLibrary.Entities
{
    public class Message : IAuditableEntity
    {
        public int ID { get; set; }
        public int ChatId { get; set; }
        public string Text { get; set; }
        public string UserNameCreator { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
