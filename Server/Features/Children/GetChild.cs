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

namespace Server.Features.Children
{
    public class GetChild
    {
        public class Query : IRequest<GetChildResponse>
        {
            public Query(int childId)
            {
                ChildId = childId;
            }

            public int ChildId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, GetChildResponse>
        {
            private readonly ApplicationContext _context;

            public QueryHandler(ApplicationContext context)
            {
                _context = context;
            }

            public async Task<GetChildResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var child = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);

                if (child== null)
                {
                    throw new ApiException("Child not found", HttpStatusCode.NotFound);
                }

                return new GetChildResponse
                {
                    Child = child
                };
            }

        }

    }
    public class GetChildResponse
    {
        public Child Child { get; set; }
    }
}
