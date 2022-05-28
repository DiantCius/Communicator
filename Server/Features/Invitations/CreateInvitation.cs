using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Features.Users;
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
    public class CreateInvitation
    {
        public class Command : IRequest<InvitationResponse>
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
        public class Handler : UserList, IRequestHandler<Command, InvitationResponse>
        {
            private readonly ApplicationContext _context;
            private readonly CurrentUser _currentUser;
            private readonly IMapper _mapper;

            public Handler(ApplicationContext context, CurrentUser currentUser, IMapper mapper)
            {
                _context = context;
                _currentUser = currentUser;
                _mapper = mapper;
            }

            public async Task<InvitationResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var child = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);

                if (child == null)
                {
                    throw new ApiException("child not found", HttpStatusCode.NotFound);
                }

                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);

                if (currentUser.PersonId != child.ParentId)
                {
                    throw new ApiException("you can't invite babysitters", HttpStatusCode.BadRequest);
                }

                var person = await _context.Persons.FirstAsync(x => x.Email == request.PersonEmail, cancellationToken);

                if (person == null)
                {
                    throw new ApiException("user not found", HttpStatusCode.NotFound);
                }

                if(person.InvitedBy == currentUser.Email)
                {
                    throw new ApiException("already invited", HttpStatusCode.BadRequest);
                }

                var invitation = new Invitation()
                {
                    AddressedUserId = person.PersonId,
                    ChildId = child.ChildId,
                    InviteDate = DateTime.UtcNow
                };

                await _context.Invitations.AddAsync(invitation, cancellationToken);

                person.InvitedBy = currentUser.Email;

                await _context.SaveChangesAsync(cancellationToken);


                var userList = await GetUsersAsync(_context, cancellationToken, request.ChildId, _mapper);

                return new InvitationResponse
                {
                    Users = userList,
                    Count = userList.Count()
                };

            }
        }
    }

    public class InvitationResponse
    {
        public List<User> Users { get; set; }
        public int Count { get; set; }
    }
}
