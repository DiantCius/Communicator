using Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Chats
{
    public class MessagesResponse
    {
        public List<Message> Messages { get; set; }
        public int Count { get; set; }

    }
}
