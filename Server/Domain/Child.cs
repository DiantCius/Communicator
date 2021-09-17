using Server.Features.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Child
    {
        public int ChildId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int ParentId { get; set; }
        [JsonIgnore]
        public List<ChildPerson> ChildPersons { get; set; } = new List<ChildPerson>();
        [JsonIgnore]
        public List<Activity> Activities { get; set; } = new List<Activity>();
    }
}
