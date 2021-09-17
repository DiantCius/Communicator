using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using Server.Infrastructure.Errors;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Babysitters
{
    public class AddBabysitter
    {
        public class Command : IRequest<AddBabysitterResponse>
        {
            public int ChildId { get; set; }
            public string PersonEmail { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
                RuleFor(x => x.PersonEmail).NotNull().NotEmpty().EmailAddress(); 
            }
        }
        public class Handler : IRequestHandler<Command, AddBabysitterResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;

            public Handler(ApplicationContext context, ICurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<AddBabysitterResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var child = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);

                if (child == null)
                {
                    throw new ApiException("child not found", HttpStatusCode.Unauthorized);
                }

                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);
                
                if(currentUser.PersonId != child.ParentId)
                {
                    throw new ApiException("only parents can add babysitters", HttpStatusCode.Unauthorized);
                }

                var person = await _context.Persons.FirstAsync(x => x.Email == request.PersonEmail, cancellationToken);

                if (person == null)
                {
                    throw new ApiException("child not found", HttpStatusCode.NotFound);
                }

                var childPerson = new ChildPerson()
                {
                    ChildId = child.ChildId,
                    Child = child,
                    PersonId = person.PersonId,
                    Person = person
                };

                await _context.ChildPersons.AddAsync(childPerson, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var babysitter = new Babysitter()
                {
                    Username = person.Username,
                    Email = person.Email
                };

                return new AddBabysitterResponse { Babysitter = babysitter };
            }
        }
    }
    public class AddBabysitterResponse
    {
        public Babysitter Babysitter { get; set; }
    }
}
