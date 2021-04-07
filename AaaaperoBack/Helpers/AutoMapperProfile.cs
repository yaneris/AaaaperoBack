using AaaaperoBack.DTO;
using AaaaperoBack.Models;
using AutoMapper;

namespace AaaaperoBack.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}