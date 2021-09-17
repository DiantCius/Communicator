using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Babysitters
{
    public class BabysitterList
    {
        public class Query : IRequest<QueryResponse>
        {
            public int ChildId { get; set; }
            public Query(int id)
            {
                ChildId = id;
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResponse>
        {
            private readonly ApplicationContext _context;

            public QueryHandler(ApplicationContext context)
            {
                _context = context;
            }

            public async Task<QueryResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = from p in _context.Persons
                            join cp in _context.ChildPersons on p.PersonId equals cp.PersonId
                            where cp.ChildId == request.ChildId
                            select p;

                var babysitters = await query.ToListAsync(cancellationToken);

                return new QueryResponse
                {
                    Babysitters = babysitters,
                    Count = babysitters.Count()
                };
            }
        }
    }
    public class QueryResponse
    {
        public List<Person> Babysitters { get; set; }
        public int Count { get; set; }
    }
}
