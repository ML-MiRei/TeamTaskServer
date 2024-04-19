using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.Common
{
    public interface IAuditableEntity : IBaseEntity
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

    }
}
