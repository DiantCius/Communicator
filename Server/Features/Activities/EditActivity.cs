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
    public class EditActivity
    {
        public class Command : IRequest<ActivityResponse>
        {
            public Command(int activityId, string action)
            {
                ActivityId = activityId;
                Action = action;
            }

            public int ActivityId { get; set; }
            public int ChildId { get; set; }
            public string Action { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ActivityId).NotNull().NotEmpty();
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
                RuleFor(x => x.Action).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, ActivityResponse>
        {
            private readonly ApplicationContext _context;

            public Handler(ApplicationContext context)
            {
                _context = context;
            }

            public async Task<ActivityResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var activityToEdit = await _context.Activities.FirstAsync(x => x.ActivityId == request.ActivityId, cancellationToken);

                if(activityToEdit == null)
                {
                    throw new ApiException($"activity with: {request.ActivityId} not found ", HttpStatusCode.NotFound);
                }

                activityToEdit.Action = request.Action ?? activityToEdit.Action;

                await _context.SaveChangesAsync(cancellationToken);

                var activities = await _context.Activities.OrderBy(x => x.ActivityId).AsNoTracking().ToListAsync(cancellationToken); // mozna do funkcji potem wyodrebnic
                var activityList = activities.Where(x => x.ChildId == request.ChildId).ToList();

                var authorList = await _context.Persons.ToListAsync(cancellationToken);

                foreach (Activity activity in activityList)
                {
                    activity.Author = authorList.Find(x => x.PersonId == activity.AuthorId);
                }

                return new ActivityResponse
                {
                    Activities = activityList,
                    Count = activityList.Count(),
                };

            }
        }

    }
}
