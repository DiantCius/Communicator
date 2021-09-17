using AutoMapper;
using Server.Domain;

namespace Server.Features.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Person, User>();
        }

    }
}
