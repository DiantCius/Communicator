using Server.Features.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Chats
{
    public class ChatUsersResponse
    {
        public List<User> Users { get; set; }
        public int Count { get; set; }
    }
}
