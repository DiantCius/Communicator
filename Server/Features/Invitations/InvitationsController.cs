using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Invitations
{
    [ApiController]
    [Route("Invitations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvitationsController
    {
        private readonly IMediator _mediator;

        public InvitationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public Task<InvitationResponse> Create([FromBody] CreateInvitation.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public Task<QueryResponse> Get(CancellationToken cancellationToken)
        {
            return _mediator.Send(new InvitationList.Query(), cancellationToken);
        }

        [HttpDelete("accept")]
        public Task<AcceptInvitationResponse> AcceptInvitation(int invitationId, string childName, CancellationToken cancellationToken)
        {
            return _mediator.Send(new AcceptInvitation.Command(invitationId, childName), cancellationToken);
        }

        [HttpDelete("decline")]
        public Task<DeclineInvitationResponse> DeclineInvitation(int invitationId, string childName, CancellationToken cancellationToken)
        {
            return _mediator.Send(new DeclineInvitation.Command(invitationId, childName), cancellationToken);
        }
    }
}
