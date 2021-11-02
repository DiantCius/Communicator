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

namespace Server.Features.Users
{
    public class UserList
    {
        public class Query : IRequest<QueryResponse>
        {
            public Query(int childId)
            {
                ChildId = childId;
            }

            public int ChildId { get; set; }
        }

        public class QueryHandler : UserList, IRequestHandler<Query, QueryResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationContext context, ICurrentUser currentUser, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<QueryResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                //returns users that are not babysitting child with specific id
                /*var query = from pe in _context.Persons
                            where !(from p in _context.Persons
                                   join cp in _context.ChildPersons on p.PersonId equals cp.PersonId
                                   where cp.ChildId == request.ChildId
                                   select p.PersonId).Contains(pe.PersonId)
                            select pe;

                var persons = await query.ToListAsync(cancellationToken);

                //var persons = await _context.Persons.OrderBy(x => x.Email).AsNoTracking().ToListAsync(cancellationToken);

                var userList = _mapper.Map<List<Person>, List<User>>(persons);*/

                var userList = await GetUsersAsync(_context, cancellationToken, request.ChildId, _mapper);


                return new QueryResponse
                {
                    Users = userList,
                    Count = userList.Count()
                };
            }

        }

        protected async Task<List<User>> GetUsersAsync(ApplicationContext applicationContext, CancellationToken cancellationToken, int childId, IMapper mapper)
        {
            var query = from pe in applicationContext.Persons
                        where !(from p in applicationContext.Persons
                                join cp in applicationContext.ChildPersons on p.PersonId equals cp.PersonId
                                where cp.ChildId == childId
                                select p.PersonId).Contains(pe.PersonId)
                        select pe;

            var persons = await query.ToListAsync(cancellationToken);

            var userList = mapper.Map<List<Person>, List<User>>(persons);

            return userList;
        }
    }

    public class QueryResponse
    {
        public List<User> Users { get; set; }
        public int Count { get; set; }
    }
}
