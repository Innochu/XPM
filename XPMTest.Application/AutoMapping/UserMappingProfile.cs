

namespace XPMTest.Application.AutoMapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserCreateDto, Customer>();
            CreateMap<Customer, UserRequestDto>();
            CreateMap<UserCreateDto, AppUser>();
            CreateMap<Customer, UserCreateDto>();
        }
    }

}
