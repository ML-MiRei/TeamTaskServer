using System.Text.RegularExpressions;
using Getaway.Core.Common;

namespace Getaway.Core.Entities
{
    public class UserEntity : IBaseEntity
    {
        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string LastName { get; set; }
        public string? Tag { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int ID { get; set; }
    }
}
