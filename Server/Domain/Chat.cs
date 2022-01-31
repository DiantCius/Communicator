using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public List<ChatPerson> ChatPersons { get; set; } 
        [JsonIgnore]
        public List<Message> Messages { get; set; } 

    }
}
