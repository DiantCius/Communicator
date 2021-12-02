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

namespace Server.Features.Babysitters
{
    public class BabysitterList
    {
        public class Query : IRequest<BabysitterResponse>
        {
            public int ChildId { get; set; }
            public Query(int id)
            {
                ChildId = id;
            }
        }

        public class QueryHandler : BabysitterList, IRequestHandler<Query, BabysitterResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<BabysitterResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var babysitterList = await GetBabysittersAsync(_context, request.ChildId, cancellationToken, _mapper);

                return new BabysitterResponse
                {
                    Babysitters = babysitterList,
                    Count = babysitterList.Count()
                };
            }

        }
        protected async Task<List<Babysitter>> GetBabysittersAsync(ApplicationContext applicationContext, int childId, CancellationToken cancellationToken, IMapper mapper)
        {
            var query = from p in applicationContext.Persons
                        join cp in applicationContext.ChildPersons on p.PersonId equals cp.PersonId
                        where cp.ChildId == childId
                        select p;

            var babysitters = await query.ToListAsync(cancellationToken);

            var babysitterList = mapper.Map<List<Person>, List<Babysitter>>(babysitters);

            return babysitterList;
        }
    }
}
