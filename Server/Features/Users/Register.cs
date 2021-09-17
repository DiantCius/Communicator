using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using Server.Infrastructure.Errors;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Users
{
    public class Register
    {
        
        public class Command : IRequest<RegisterResponse>
        {
            public string Username { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
                RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, RegisterResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IPasswordHasher<Person> _hasher;
            private readonly IJwtService _jwtService;
            public Handler(ApplicationContext context, IPasswordHasher<Person> hasher ,IJwtService jwtService)
            {
                _context = context;
                _hasher = hasher;
                _jwtService = jwtService;
            }
            public async Task<RegisterResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _context.Persons.Where(x => x.Username == request.Username).AnyAsync(cancellationToken))
                {
                    throw new ApiException($"User with username: {request.Username}  already exists", HttpStatusCode.BadRequest);
                }

                if (await _context.Persons.Where(x => x.Email == request.Email).AnyAsync(cancellationToken))
                {
                    throw new ApiException($"User with username: {request.Email}  already exists", HttpStatusCode.BadRequest);
                }

                var newPerson = new Person()
                {
                    Username = request.Username,
                    Email = request.Email,
                };

                var token = _jwtService.GenerateToken(request.Username);

                var passwordHash = _hasher.HashPassword(newPerson, request.Password);
                newPerson.HashedPassword = passwordHash;

                await _context.Persons.AddAsync(newPerson, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var user = new User()
                {
                    Username = newPerson.Username,
                    Email = newPerson.Email,
                    Token = token
                };

                return new RegisterResponse
                {
                    User = user
                };

            }
        }
 
    }
    public class RegisterResponse
    {
        public User User { get; set; }
    }
}
