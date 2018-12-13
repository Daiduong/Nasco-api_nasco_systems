using AutoMapper;
using System.Collections.Generic;

namespace NascoWebAPI.Helper.AutoMapperHelper
{
    public class AutoMapperProfileConfiguration : Profile
    {
        private void ConfiguationMenu()
        {
            //CreateMap<MenuDetailViewModel, MenuViewModel>();
            //CreateMap<MenuViewModel, MenuDetailViewModel>();
        }        
        private void ConfiguationRole()
        {
            //CreateMap<RoleDetailViewModel, RoleViewModel>();
            //CreateMap<RoleViewModel, RoleDetailViewModel>();

            //CreateMap<Category, CategoryDetailViewModel>()
            //    .ForMember(x => x.Parent,opt => opt.Ignore());
        }
        

    }
}
