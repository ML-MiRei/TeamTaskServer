using Getaway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getaway.Application.ReturnsModels
{
    public class ChatModel
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public int UserRole {  get; set; }
        public int Type { get; set; }
        public string Image { get; set; }
        public List<UserModel> Users { get; set; }
        public List<MessageModel> Messages { get; set; }

        public int ColorNumber { get; set; }


    }
}
