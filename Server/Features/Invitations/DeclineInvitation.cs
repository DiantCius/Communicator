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

namespace Server.Features.Invitations
{
    public class DeclineInvitation
    {
        public class Command : IRequest<DeclineInvitationResponse>
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

        public class Handler : InvitationList, IRequestHandler<Command, DeclineInvitationResponse>
        {
            private readonly ApplicationContext _context;
            private readonly CurrentUser _currentUser;

            public Handler(ApplicationContext context, CurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<DeclineInvitationResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var invitation = await _context.Invitations.FirstAsync(x => x.InvitationId == request.InvitationId, cancellationToken);

                if (invitation == null)
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

                

                _context.Invitations.Remove(invitation);

                currentUser.InvitedBy = null;

                await _context.SaveChangesAsync(cancellationToken);


                var invitations = await GetInviteDetailsAsync(_currentUser, _context, cancellationToken);

                return new DeclineInvitationResponse
                {
                    Invitations = invitations,
                    Count = invitations.Count
                };

            }
        }
    }
    public class DeclineInvitationResponse
    {
        public List<InviteDetails> Invitations { get; set; }
        public int Count { get; set; }
    }
}

