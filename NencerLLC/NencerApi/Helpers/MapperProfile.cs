using AutoMapper;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NencerApi.Modules.SystemNc.Model;
using NencerApi.Modules.SystemNc.Model.DTO;
using NencerApi.Modules.User.Model;

namespace NencerApi.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserModel, UserReqDto>().ReverseMap();
        }
    }
}
