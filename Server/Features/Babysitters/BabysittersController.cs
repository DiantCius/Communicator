using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Babysitters
{
    [ApiController]
    [Route("Babysitters")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BabysittersController
    {
        private readonly IMediator _mediator;

        public BabysittersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add")]
        public Task<AddBabysitterResponse> Create([FromBody] AddBabysitter.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public Task<BabysitterResponse> Get(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new BabysitterList.Query(id), cancellationToken);
        }

        [HttpDelete("delete")]
        public Task<BabysitterResponse> Delete(string babysitterUsername, int childId, CancellationToken cancellationToken)
        {
            return _mediator.Send(new DeleteBabysitter.Command(babysitterUsername, childId), cancellationToken);
        }

    }
}
