using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Children
{
    [ApiController]
    [Route("Children")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChildrenController
    {
        private readonly IMediator _mediator;

        public ChildrenController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add")]
        public Task<ChildrenResponse> Create([FromBody] AddChild.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public Task<ChildrenResponse> Get(CancellationToken cancellationToken)
        {
            return _mediator.Send(new ChildList.Query(), cancellationToken);
        }

        [HttpDelete("delete")]
        public Task<ChildrenResponse> Delete(int childId, CancellationToken cancellationToken)
        {
            return _mediator.Send(new DeleteChild.Command(childId), cancellationToken);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id:int}")]
        public Task<GetChildResponse> GetById(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new GetChild.Query(id), cancellationToken);
        }
    }
}
