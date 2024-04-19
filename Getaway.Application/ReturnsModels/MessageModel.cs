using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.ReturnsModels
{
    public class MessageModel
    {
        public string CreatorTag {  get; set; }
        public int MessageId { get; set; }
        public string TextMessage { get; set; }
        public string UserNameCreator { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
