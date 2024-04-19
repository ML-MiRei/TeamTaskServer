using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Common;
using ModelsLibrary.Enums;

namespace ModelsLibrary.Entities
{


    internal class Notification : IAuditableEntity
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public NotificationType Type { get; set; }
        public object Subject { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int ID { get; set; }
    }
}
