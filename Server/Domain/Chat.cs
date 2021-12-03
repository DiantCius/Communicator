using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string Name { get; set; }
        public int PersonId { get; set; }
        public List<ChatPerson> ChatPersons { get; set; } = new List<ChatPerson>();
        public List<Message> Messages { get; set; } = new List<Message>();

    }
}
