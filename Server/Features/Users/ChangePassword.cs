using AutoMapper;
using FluentEmail.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using Server.Domain;
using Server.Infrastructure;
using Server.Infrastructure.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Users
{
    public class ChangePassword
    {
        public class Command : IRequest<ChangePasswordResponse>
        {
            public string Email { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
            }
        }

        public class Handler : IRequestHandler<Command, ChangePasswordResponse>
        {
            private readonly ApplicationContext _context;
            private readonly IMapper _mapper;
            private readonly IPasswordHasher<Person> _hasher;
            private readonly IFluentEmail _fluentEmail;

            public Handler(ApplicationContext context,  IMapper mapper, IPasswordHasher<Person> hasher, IFluentEmail fluentEmail)
            {
                _context = context;
                _mapper = mapper;
                _hasher = hasher;
                _fluentEmail = fluentEmail;
            }

            public async Task<ChangePasswordResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

                if (person == null)
                {
                    throw new ApiException($"user with this email not found", HttpStatusCode.NotFound);
                }

                var pwd = new Password();
                var result = pwd.Next();

                var passwordHash = _hasher.HashPassword(person, result);

                person.HashedPassword = passwordHash ?? person.HashedPassword;

                await _context.SaveChangesAsync(cancellationToken);

                SendSingleEmail(person.Email, result);

                var user = _mapper.Map<Person, User>(person);

                return new ChangePasswordResponse
                {
                    User = user
                };
            }
            private async void SendSingleEmail( string userEmail, string password)
            {
                var email = _fluentEmail
                    .To(userEmail)
                    .Subject("Password change")
                    .Body($"Here is your new password: {password}");
                try
                {
                    await email.SendAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }
        
    }
    public class ChangePasswordResponse
    {
        public User User { get; set; }
    }
}

