using MediatR;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Server.Features.Invitations
{
    public class InvitationList
    {
        public class Query : IRequest<QueryResponse>
        {

        }

        public class QueryHandler : InvitationList, IRequestHandler<Query, QueryResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;

            public QueryHandler(ApplicationContext context, ICurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;

            }

            public async Task<QueryResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                /*var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);

                var invitations = await _context.Invitations.OrderBy(x => x.InvitationId).AsNoTracking().ToListAsync(cancellationToken);
                var invitationList = invitations.Where(x => x.AddressedUserId == currentUser.PersonId).ToList();

                var invitationsDeatils = new List<InviteDetails>();

                foreach(var invite in invitationList)
                {
                    var child = await _context.Children.FirstAsync(x => x.ChildId == invite.ChildId, cancellationToken);
                    invitationsDeatils.Add(new InviteDetails { ChildName = child.Name, InviteDate = invite.InviteDate });
                }*/

                var invitationDetails = await GetInviteDetailsAsync(_currentUser, _context, cancellationToken);

                return new QueryResponse
                {
                    Invitations = invitationDetails,
                    Count = invitationDetails.Count
                };
            }
        }

        protected async Task<List<InviteDetails>>GetInviteDetailsAsync(ICurrentUser _currentUser, ApplicationContext applicationContext, CancellationToken cancellationToken)
        {
            var currentUserUsername = _currentUser.GetCurrentUsername();
            var currentUser = await applicationContext.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);

            var invitations = await applicationContext.Invitations.OrderBy(x => x.InvitationId).AsNoTracking().ToListAsync(cancellationToken);
            var invitationList = invitations.Where(x => x.AddressedUserId == currentUser.PersonId).ToList();

            var invitationsDeatils = new List<InviteDetails>();

            foreach (var invite in invitationList)
            {
                var child = await applicationContext.Children.FirstAsync(x => x.ChildId == invite.ChildId, cancellationToken);
                invitationsDeatils.Add(new InviteDetails { InvitationId = invite.InvitationId, ChildName = child.Name, InviteDate = invite.InviteDate });
            }

            return invitationsDeatils;

        }

    }
    public class QueryResponse
    {
        public List<InviteDetails> Invitations { get; set; }
        public int Count { get; set; }
    }
}
