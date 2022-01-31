using Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Chats
{
    public class ChatsResponse
    {
        public List<Chat> Chats { get; set; }
        public int Count { get; set; }
    }
}
