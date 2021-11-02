using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Features.Users;
using Server.Infrastructure;
using Server.Infrastructure.Errors;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


using System;

using System.Linq;

namespace Server.Features.Babysitters
{
    public class AddBabysitter
    {
        public class Command : IRequest<AddBabysitterResponse>
        {
            public int ChildId { get; set; }
            public string PersonEmail { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ChildId).NotNull().NotEmpty();
                RuleFor(x => x.PersonEmail).NotNull().NotEmpty().EmailAddress(); 
            }
        }
        public class Handler : UserList, IRequestHandler<Command, AddBabysitterResponse>
        {
            private readonly ApplicationContext _context;
            private readonly ICurrentUser _currentUser;
            private readonly IMapper _mapper;

            public Handler(ApplicationContext context, ICurrentUser currentUser, IMapper mapper)
            {
                _context = context;
                _currentUser = currentUser;
                _mapper = mapper;
            }

            public async Task<AddBabysitterResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var child = await _context.Children.FirstAsync(x => x.ChildId == request.ChildId, cancellationToken);

                if (child == null)
                {
                    throw new ApiException("child not found", HttpStatusCode.NotFound);
                }

                var currentUserUsername = _currentUser.GetCurrentUsername();
                var currentUser = await _context.Persons.FirstAsync(x => x.Username == currentUserUsername, cancellationToken);
                
                if(currentUser.PersonId != child.ParentId)
                {
                    throw new ApiException("only parents can add babysitters", HttpStatusCode.BadRequest);
                }

                var person = await _context.Persons.FirstAsync(x => x.Email == request.PersonEmail, cancellationToken);

                if (person == null)
                {
                    throw new ApiException("child not found", HttpStatusCode.NotFound);
                }

                var childPerson = new ChildPerson()
                {
                    ChildId = child.ChildId,
                    Child = child,
                    PersonId = person.PersonId,
                    Person = person
                };

                await _context.ChildPersons.AddAsync(childPerson, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // zwrocenie listy uzytkownikow ktorzy nie opiekuja sie dzieckiem

                /*var query = from pe in _context.Persons
                            where !(from p in _context.Persons
                                    join cp in _context.ChildPersons on p.PersonId equals cp.PersonId
                                    where cp.ChildId == request.ChildId
                                    select p.PersonId).Contains(pe.PersonId)
                            select pe;

                var persons = await query.ToListAsync(cancellationToken);


                var userList = _mapper.Map<List<Person>, List<User>>(persons);*/

                var userList = await GetUsersAsync(_context, cancellationToken, request.ChildId, _mapper);

                return new AddBabysitterResponse
                {
                    Users = userList,
                    Count = userList.Count()
                };

                /*var babysitter = new Babysitter()
                {
                    Username = person.Username,
                    Email = person.Email
                };

                return new AddBabysitterResponse { Babysitter = babysitter };*/
            }
        }
    }
    /*public class AddBabysitterResponse
    {
        public Babysitter Babysitter { get; set; }
    }*/

    public class AddBabysitterResponse
    {
        public List<User> Users { get; set; }
        public int Count { get; set; }
    }


}
