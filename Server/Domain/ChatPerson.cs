using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class ChatPerson
    {
        public int PersonId { get; set; }
        public Person Person{ get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
