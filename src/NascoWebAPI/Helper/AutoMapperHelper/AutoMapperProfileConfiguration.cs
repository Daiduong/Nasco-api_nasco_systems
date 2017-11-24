using AutoMapper;
using System.Collections.Generic;

namespace NascoWebAPI.Helper.AutoMapperHelper
{
    public class AutoMapperProfileConfiguration : Profile
    {
#pragma warning disable CS0672 // Member overrides obsolete member
        protected override void Configure()
#pragma warning restore CS0672 // Member overrides obsolete member
        {
            ConfiguationMenu();
            ConfiguationRole();
           
        }
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
