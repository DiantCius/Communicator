using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public DateTime WhenSent { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
