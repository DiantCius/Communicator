using System;

namespace Server.Domain
{
    public class Invitation
    {
        public int InvitationId { get; set; }
        public int AddressedUserId { get; set; }
        public int ChildId { get; set; }
        public DateTime InviteDate { get; set; }


    }
}
