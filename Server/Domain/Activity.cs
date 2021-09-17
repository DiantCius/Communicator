using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string Action { get; set; }
        public DateTime PostTime { get; set; }
        public Person Author { get; set; }
        [JsonIgnore]
        public int AuthorId { get; set; }
        [JsonIgnore]
        public Child Child { get; set; }
        [JsonIgnore]
        public int ChildId { get; set; }
    }
}
