using MediatR;
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
        public Task<QueryResponse> Get(int id, CancellationToken cancellationToken)
        {
            return _mediator.Send(new UserList.Query(id), cancellationToken);
        }

    }
}
