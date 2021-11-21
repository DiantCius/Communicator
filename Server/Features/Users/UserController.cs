using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Users
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("User")]
    public class UserController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("details")]
        public Task<GetUserResponse> Get(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new GetUser.Query(id), cancellationToken);
        }

        [HttpGet]
        public Task<CurrentUserResponse> GetById(CancellationToken cancellationToken)
        {
            return _mediator.Send(new CurrentUserDetails.Query(), cancellationToken);
        }

    }
}
