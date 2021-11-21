using AutoMapper;
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

namespace Server.Features.Babysitters
{
    public class DeleteBabysitter
    {
        public class Command : IRequest<BabysitterResponse>
        {
            public Command(string babysitterUsername, int childId)
            {
                BabysitterUsername =  babysitterUsername;
                ChildId = childId;
            }

            public string BabysitterUsername { get; set; }
            public int ChildId { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.BabysitterUsername).NotNull().NotEmpty();
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
            }
        }

        public class Handler : BabysitterList, IRequestHandler<Command, BabysitterResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;
            private readonly IMapper _mapper;

            public Handler(ApplicationContext context, ICurrentUser currentUser, IMapper mapper)
            {
                _context = context;
                _currentUser = currentUser;
                _mapper = mapper;
            }

            public async Task<BabysitterResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

                var child = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);

                if(child == null)
                {
                    throw new ApiException("child not found ", HttpStatusCode.NotFound);
                }

                if (child.ParentId != currentUser.PersonId)
                {
                    throw new ApiException("You can't delete this babysitter", HttpStatusCode.BadRequest);
                }

                var personToDelete = await _context.Persons.FirstAsync(x => x.Username == request.BabysitterUsername, cancellationToken);
                var babysitterId = personToDelete.PersonId;

                if(currentUser.PersonId == babysitterId)
                {
                    throw new ApiException("You can't delete yourself", HttpStatusCode.BadRequest);
                }

                var babysitterToDelete = await _context.ChildPersons.FirstAsync(x => x.PersonId == babysitterId && x.ChildId == request.ChildId, cancellationToken);

                if (babysitterToDelete == null)
                {
                    throw new ApiException("babysitter  not found ", HttpStatusCode.NotFound);
                }

                _context.ChildPersons.Remove(babysitterToDelete);

                personToDelete.InvitedBy = null;

                await _context.SaveChangesAsync(cancellationToken);

                var babysitterList = await GetBabysittersAsync(_context, request.ChildId, cancellationToken, _mapper);

                return new BabysitterResponse
                {
                    Babysitters = babysitterList,
                    Count = babysitterList.Count()
                };
            }
        }
    }
}
