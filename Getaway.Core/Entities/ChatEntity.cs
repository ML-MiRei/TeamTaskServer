using Getaway.Core.Common;

namespace Getaway.Core.Entities
{


    public class ChatEntity : IBaseEntity
    {
        public int ID { get; set; }
        public string ChatName { get; set; }
        public int Type { get; set; }
        public int? AdminId { get; set; }
        public string? AdminTag { get; set; }
        public string Image { get; set; }

    }
}
