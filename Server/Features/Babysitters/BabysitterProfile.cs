using AutoMapper;
using Server.Domain;

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
