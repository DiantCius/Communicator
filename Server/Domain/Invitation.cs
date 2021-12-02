using System;

namespace Server.Domain
{
    public class Invitation
    {
        public int InvitationId { get; set; }
        public Person AddressedUser { get; set; }
        public int AddressedUserId { get; set; }
        public Child Child { get; set; }
        public int ChildId { get; set; }
        public DateTime InviteDate { get; set; }


    }
}
