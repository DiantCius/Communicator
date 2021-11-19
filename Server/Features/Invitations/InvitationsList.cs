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
    public class InvitationsList
    {
        public class Query : IRequest<QueryResponse>
        {

        }

        public class QueryHandler : IRequestHandler<Query, QueryResponse>
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
                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);

                var invitations = await _context.Invitations.OrderBy(x => x.InvitationId).AsNoTracking().ToListAsync(cancellationToken);
                var invitationList = invitations.Where(x => x.AddressedUserId == currentUser.PersonId).ToList();

                return new QueryResponse
                {
                    Invitations = invitationList,
                    Count = invitationList.Count
                };
            }
        }

    }
    public class QueryResponse
    {
        public List<Invitation> Invitations { get; set; }
        public int Count { get; set; }
    }
}
