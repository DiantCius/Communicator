using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Users
{
    [ApiController]
    [Route ("Users")]
    public class UsersController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public Task<RegisterResponse> Create([FromBody] Register.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpPost("login")]
        public Task<LoginResponse> Login([FromBody] Login.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public Task<QueryResponse> GetUsers(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new UserList.Query(id), cancellationToken);
        }

        [HttpGet("details")]
        public Task<GetUserResponse> GetUser(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new GetUser.Query(id), cancellationToken);
        }

        [HttpGet("user")]
        public Task<CurrentUserResponse> GetCurrentUser(CancellationToken cancellationToken)
        {
            return _mediator.Send(new CurrentUserDetails.Query(), cancellationToken);
        }

        [AllowAnonymous]
        [HttpPost("password")]
        public Task<ChangePasswordResponse> ChangePassword(ChangePassword.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }
    }
}
