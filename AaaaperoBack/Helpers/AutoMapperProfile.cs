using AaaaperoBack.DTO;
using AaaaperoBack.Models;
using AutoMapper;
using DTO;

namespace Helpers
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