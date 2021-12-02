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

namespace Server.Features.Activities
{
    public class AddActivity
    {
        public class Command : IRequest<ActivityResponse>
        {
            public string Action { get; set; }
            public int ChildId { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Action).NotNull().NotEmpty();
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
            }
        }
        public class Handler : ActivityList, IRequestHandler<Command, ActivityResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;

            public Handler(ApplicationContext context, ICurrentUser currentUser)
            {
                _context = context;
                _currentUser = currentUser;
            }

            public async Task<ActivityResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var child = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);

                if (child == null)
                {
                    throw new ApiException($"child with {request.ChildId} not found", HttpStatusCode.NotFound);
                }

                var activityAuthor = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

                var babysitter = await _context.ChildPersons.FirstOrDefaultAsync(x => x.ChildId == request.ChildId && x.PersonId == activityAuthor.PersonId);

                if (babysitter == null)
                {
                    throw new ApiException("only babysitters can add activities to children", HttpStatusCode.Unauthorized);
                }

                var newActivity = new Activity()
                {
                    Action = request.Action,
                    PostTime = DateTime.UtcNow,
                    Author = activityAuthor,
                    Child = child,
                };

                await _context.Activities.AddAsync(newActivity, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var activityList = await GetActivitiesAsync(_context, cancellationToken, request.ChildId);

                return new ActivityResponse
                {
                    Activities = activityList,
                    Count = activityList.Count(),
                };

            }
        }
    }

}
