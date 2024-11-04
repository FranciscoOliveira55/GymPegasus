using AutoMapper;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace BackEnd.Configurations
{
    public class MapperInitilizer : Profile
    {
        public MapperInitilizer()
        {
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<User, UserRegisterBODTO>().ReverseMap();
            CreateMap<User, UserQueryDTO>();
            CreateMap<User, UserQueryDTO>().ReverseMap();
        }
    }
}
