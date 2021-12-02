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
    public class DeleteActivity
    {
        public class Command : IRequest<ActivityResponse>
        {
            public Command(int activityId, int childId)
            {
                ActivityId = activityId;
                ChildId = childId;
            }

            public int ActivityId { get; set; }
            public int ChildId { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ActivityId).NotNull().NotEmpty();
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
            }
        }

        public class Handler : ActivityList, IRequestHandler<Command, ActivityResponse>
        {
            private readonly ApplicationContext _context;

            public Handler(ApplicationContext context)
            {
                _context = context;
            }

            public async Task<ActivityResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var activityToDelete = await _context.Activities.FirstAsync(x => x.ActivityId == request.ActivityId, cancellationToken);
                if(activityToDelete == null)
                {
                    throw new ApiException($"activity with: {request.ActivityId} not found ", HttpStatusCode.NotFound);
                }

                _context.Activities.Remove(activityToDelete);
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
