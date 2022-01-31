using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Users
{
    public class CurrentUserDetails
    {
        public class Query : IRequest<CurrentUserResponse>
        {
        }

        public class QueryHandler : IRequestHandler<Query, CurrentUserResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IMapper _mapper;
            private readonly CurrentUser _currentUser;

            public QueryHandler(ApplicationContext context, IMapper mapper, CurrentUser currentUser)
            {
                _context = context;
                _mapper = mapper;
                _currentUser = currentUser;
            }

            public async Task<CurrentUserResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentPerson = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);

                var currentUser = _mapper.Map<Person, User>(currentPerson);

                return new CurrentUserResponse
                {
                    User = currentUser
                };
            }

        }

    }
    public class CurrentUserResponse
    {
        public User User { get; set; }
    }

}
