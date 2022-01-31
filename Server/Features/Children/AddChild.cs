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
        public class Command : IRequest<ChildrenResponse>
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
        public class Handler : ChildList, IRequestHandler<Command, ChildrenResponse>
        {
            private readonly ApplicationContext _context;
            private readonly CurrentUser _currentUser;

            public Handler(ApplicationContext context, CurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<ChildrenResponse> Handle(Command request, CancellationToken cancellationToken)
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

                /*return new AddChildResponse
                {
                    Child = newChild
                };*/

                /*var query = from c in _context.Children
                            join ca in _context.ChildPersons on c.ChildId equals ca.ChildId
                            where ca.PersonId == parent.PersonId
                            select c;

                var children = await query.ToListAsync(cancellationToken);*/

                var children = await GetChildrenAsync(_context, parent, cancellationToken);

                return new ChildrenResponse
                {
                    Children = children,
                    Count = children.Count()
                };
            }
        }
    }
    /*public class AddChildResponse 
    {
        public Child Child { get; set; }
    }*/
}
