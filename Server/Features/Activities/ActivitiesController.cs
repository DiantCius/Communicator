using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Activities
{
    [ApiController]
    [Route("Activities")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ActivitiesController
    {
        private readonly IMediator _mediator;

        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add")]
        public Task<ActivityResponse> Create([FromBody] AddActivity.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public Task<ActivityResponse> Get(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new ActivityList.Query(id), cancellationToken);
        }

        [HttpDelete("delete")]
        public Task<ActivityResponse> Delete(int activityId, int childId, CancellationToken cancellationToken)
        {
            return _mediator.Send(new DeleteActivity.Command(activityId, childId), cancellationToken);
        }

        [HttpPut("edit")]
        public Task<ActivityResponse> Edit([FromBody] EditActivity.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

    }
}
