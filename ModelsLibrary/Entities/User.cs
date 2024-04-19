using System.Text.RegularExpressions;
using ModelsLibrary.Common;

namespace ModelsLibrary.Entities
{
    public class User : IAuditableEntity
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Tag { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int ID { get; set; }
    }
}
