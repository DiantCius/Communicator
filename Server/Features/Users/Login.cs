using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Server.Infrastructure.Errors;
using AutoMapper;

namespace Server.Features.Users
{
    public class Login
    {
        public class Command : IRequest<LoginResponse>
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, LoginResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IPasswordHasher<Person> _hasher;
            private readonly IJwtService _jwtService;
            private readonly IMapper _mapper;

            public Handler(ApplicationContext context, IPasswordHasher<Person> hasher, IJwtService jwtService, IMapper mapper)
            {
                _context = context;
                _hasher = hasher;
                _jwtService = jwtService;
                _mapper = mapper;
            }
            public async Task<LoginResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.Where(x => x.Email == request.Email).SingleOrDefaultAsync(cancellationToken);
                if(person == null)
                {
                    throw new ApiException("Invalid email/password", HttpStatusCode.BadRequest);
                }
                var result = _hasher.VerifyHashedPassword(person, person.HashedPassword, request.Password);
                if(result == 0)
                {
                    throw new ApiException("Invalid email/password", HttpStatusCode.BadRequest);
                }
                var token =_jwtService.GenerateToken(person.Username);
                var user = _mapper.Map<Person, User>(person);
                user.Token = token;

                return new LoginResponse
                {
                    User = user,
                };
            }
        }
    }
    public class LoginResponse 
    {
        public User User { get; set; }
    }

}
