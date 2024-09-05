using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace AutoConsa.Reportes.Presentacion
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //UrlFriendly
            ConfigurarRutas();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
           
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        private void ConfigurarRutas()
        {
            RouteTable.Routes.MapPageRoute("generico", "", "~/rptReportesSIAC.aspx");
            RouteTable.Routes.MapPageRoute("principal", "Default", "~/rptReportesSIAC.aspx");
            RouteTable.Routes.MapPageRoute("reportes", "Reportes", "~/rptReportesSIAC.aspx");
            RouteTable.Routes.MapPageRoute("visor", "Visor", "~/frmVisor.aspx");
        }
    }
}