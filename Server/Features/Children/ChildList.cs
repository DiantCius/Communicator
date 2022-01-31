
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Children
{
    public class ChildList
    {
        public class Query : IRequest<ChildrenResponse>
        {
        }

        public class QueryHandler : ChildList, IRequestHandler<Query, ChildrenResponse>
        {
            private readonly ApplicationContext _context;
            private readonly CurrentUser _currentUser;

            public QueryHandler(ApplicationContext context, CurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<ChildrenResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);

                var children = await GetChildrenAsync(_context, currentUser, cancellationToken);

                return new ChildrenResponse
                {
                    Children = children,
                    Count = children.Count()
                };
                
            }
        }

        public async Task<List<Child>> GetChildrenAsync(ApplicationContext applicationContext, Person currentUser, CancellationToken cancellationToken )
        {
            var query = from c in applicationContext.Children
                        join ca in applicationContext.ChildPersons on c.ChildId equals ca.ChildId
                        where ca.PersonId == currentUser.PersonId
                        select c;

            var children = await query.ToListAsync(cancellationToken);

            return children;
        }
    }

}
