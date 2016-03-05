using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Akka.Actor;
using Cellular.Actors;

namespace Cellular
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static ActorSystem System;
        public static IActorRef Ecosystem;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System = ActorSystem.Create("cellular");
            Ecosystem = System.ActorOf<EcosystemActor>("ecosystem");
        }

        protected void Application_End()
        {
            System.Terminate();
        }
    }
}
