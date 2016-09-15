using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ProcessAccelerator.Infra;

namespace ProcessAccelerator.WebUI
{
    public class Bootstrapper
    {
        public static void Bootstrap()
        {
            RouteConfigurator.RegisterRoutes(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IoC.Container));
            WindsorConfigurator.Configure();
            //AwesomeConfigurator.Configure(); --> User when enabling Localisation

            Globals.PicturesPath = HttpContext.Current.Server.MapPath("~/pictures");
            Globals.ErrorPage = "/Main/Unauthorized";
            Globals.DateFormatString = "dd/MM/yyyy";
            Globals.DateFormatValue = "{0:dd/MM/yyyy}";
            //new  Worker().Start();
        }
    }
}