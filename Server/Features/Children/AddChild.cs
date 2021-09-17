using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Children
{
    public class AddChild
    {
        public class Command : IRequest<AddChildResponse>
        {
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Name).NotNull().NotEmpty();
                RuleFor(x => x.BirthDate).NotNull().NotEmpty();
            }
        }
        public class Handler : IRequestHandler<Command, AddChildResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;

            public Handler(ApplicationContext context, ICurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<AddChildResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var parent = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

                var newChild = new Child()
                {
                    Name = request.Name,
                    BirthDate = request.BirthDate,
                    ParentId = parent.PersonId
                };

                var childPerson = new ChildPerson()
                {
                    ChildId = newChild.ChildId,
                    Child = newChild,
                    PersonId = parent.PersonId,
                    Person = parent
                };

                await _context.ChildPersons.AddAsync(childPerson, cancellationToken);
                await _context.Children.AddAsync(newChild, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new AddChildResponse
                {
                    Child = newChild
                };
            }
        }
    }
    public class AddChildResponse 
    {
        public Child Child { get; set; }
    }
}
