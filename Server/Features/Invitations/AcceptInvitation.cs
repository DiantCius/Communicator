using FluentValidation;
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

namespace Server.Features.Invitations
{
    public class AcceptInvitation
    {
        public class Command : IRequest<AcceptInvitationResponse>
        {
            public Command(int invitationId, string childName)
            {
                InvitationId = invitationId;
                ChildName = childName;
            }

            public int InvitationId { get; set; }
            public string ChildName { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.InvitationId).NotNull().NotEmpty();
                RuleFor(x => x.ChildName).NotNull().NotEmpty();
            }
        }

        public class Handler : InvitationList, IRequestHandler<Command, AcceptInvitationResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;

            public Handler(ApplicationContext context, ICurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<AcceptInvitationResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var invitation = await _context.Invitations.FirstAsync(x => x.InvitationId == request.InvitationId, cancellationToken);

                if(invitation == null)
                {
                    throw new ApiException("invitation not found", HttpStatusCode.NotFound);
                }

                var child = await _context.Children.FirstAsync(x => x.Name == request.ChildName, cancellationToken);

                if (child == null)
                {
                    throw new ApiException("child not found", HttpStatusCode.NotFound);
                }

                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);


                var childPerson = new ChildPerson()
                {
                    ChildId = child.ChildId,
                    Child = child,
                    PersonId = currentUser.PersonId,
                    Person = currentUser
                };


                _context.Invitations.Remove(invitation);

                await _context.ChildPersons.AddAsync(childPerson, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);


                var invitations = await GetInviteDetailsAsync(_currentUser, _context, cancellationToken);

                return new AcceptInvitationResponse
                {
                    Invitations = invitations,
                    Count = invitations.Count
                };

            }
        }
    }
    public class AcceptInvitationResponse
    {
        public List<InviteDetails> Invitations { get; set; }
        public int Count { get; set; }
    }
}
