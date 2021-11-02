using AutoMapper;
using Server.Domain;
using Server.Features.Users;

namespace Server.Features.Babysitters
{
    public class BabysitterProfile : Profile
    {
        public BabysitterProfile()
        {
            CreateMap<Person, Babysitter>();
        }

    }
}
