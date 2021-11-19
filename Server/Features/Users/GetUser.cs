using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using Server.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Users
{
    public class GetUser
    {
        public class Query : IRequest<GetUserResponse>
        {
            public Query(int userId)
            {
                UserId = userId;
            }

            public int UserId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, GetUserResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<GetUserResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.FirstAsync(x => x.PersonId == request.UserId, cancellationToken);
                
                if (person == null)
                {
                    throw new ApiException("User not found", HttpStatusCode.NotFound);
                }

                var user = _mapper.Map<Person, User>(person);

                return new GetUserResponse
                {
                    User = user
                };
            }

        }
        
    }
    public class GetUserResponse
    {
        public User User { get; set; }
    }


}
