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
        public Task<AddChildResponse> Create([FromBody] AddChild.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public Task<QueryResponse> Get(CancellationToken cancellationToken)
        {
            return _mediator.Send(new ChildList.Query(), cancellationToken);
        }
    }
}
