using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Getaway.Core.Common;
using Getaway.Core.Enums;

namespace Getaway.Core.Entities
{


    public class NotificationEntity : IBaseEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }

    }
}
