using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class DeleteChild
    {
        public class Command : IRequest<ChildrenResponse>
        {
            public Command(int childId)
            {
                ChildId = childId;
            }
            public int ChildId { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
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

                var currentUser = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

                var childToDelete = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);
                if (childToDelete == null)
                {
                    throw new ApiException($"activity with: {request.ChildId} not found ", HttpStatusCode.NotFound);
                }

                if(childToDelete.ParentId != currentUser.PersonId)
                {
                    throw new ApiException("You cant add children", HttpStatusCode.BadRequest);
                }

                _context.Children.Remove(childToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                var children = await GetChildrenAsync(_context, currentUser, cancellationToken);

                return new ChildrenResponse
                {
                    Children = children,
                    Count = children.Count()
                };
            }

        }
    }
}
