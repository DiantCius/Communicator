using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsInvited { get; set; } = false;
        [JsonIgnore]
        public string HashedPassword { get; set; }
        [JsonIgnore]
        public List<ChildPerson> ChildPersons { get; set; } = new List<ChildPerson>();

    }
}
