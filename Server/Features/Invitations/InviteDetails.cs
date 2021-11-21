using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Invitations
{
    public class InviteDetails
    {
        public int InvitationId { get; set; }
        public string ChildName { get; set; }
        public DateTime InviteDate { get; set; }
    }
}
