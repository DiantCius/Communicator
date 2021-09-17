using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Children
{
    public class ChildList
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

                var query = from c in _context.Children
                            join ca in _context.ChildPersons on c.ChildId equals ca.ChildId
                            where ca.PersonId == currentUser.PersonId
                            select c;

                var children = await query.ToListAsync(cancellationToken);

                return new QueryResponse
                {
                    Children = children,
                    Count = children.Count()
                };
                
            }
        }
    }

    public class QueryResponse
    {
        public List<Child> Children { get; set; }
        public int Count { get; set; }
    }
}
