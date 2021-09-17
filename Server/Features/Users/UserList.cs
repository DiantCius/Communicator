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
        }

        public class QueryHandler : IRequestHandler<Query, QueryResponse>
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
                var persons = await _context.Persons.OrderBy(x => x.Email).AsNoTracking().ToListAsync(cancellationToken);

                var userList = _mapper.Map<List<Person>, List<User>>(persons);

                return new QueryResponse
                {
                    Users = userList,
                    Count = userList.Count()
                };
            }
        }
    }

    public class QueryResponse
    {
        public List<User> Users { get; set; }
        public int Count { get; set; }
    }
}
