using Omu.Encrypto;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Security;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Infra;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.WebUI.Controllers;
using ProcessAccelerator.WebUI.Dto;

namespace ProcessAccelerator.WebUI
{
    public class WindsorConfigurator
    {
        public static void Configure()
        {
            // WindsorRegistrar.Register(typeof(IFormsAuthentication), typeof(FormAuthService));
            WindsorRegistrar.Register(typeof(IHasher), typeof(Hasher));
            WindsorRegistrar.Register(typeof(IMapper<mstr_process_level, mstr_process_levelInput>), typeof(mstr_process_levelMapper));
            WindsorRegistrar.Register(typeof(IMapper<mstr_process_role, mstr_process_roleInput>), typeof(mstr_process_roleMapper));
            //WindsorRegistrar.Register(typeof(IUserService), typeof(UserService));
            //WindsorRegistrar.Register(typeof(IMealService), typeof(MealService));

            WindsorRegistrar.RegisterAllFromAssemblies("ProcessAccelerator.Data");
            WindsorRegistrar.RegisterAllFromAssemblies("ProcessAccelerator.Service");
            WindsorRegistrar.RegisterAllFromAssemblies("ProcessAccelerator.WebUI");
        }
    }
}