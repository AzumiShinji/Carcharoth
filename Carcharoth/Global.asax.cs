using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Carcharoth
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //var g = new CookiesClass();
            //if (g.ReadCookie("IsFirstRun") == null)
            //    g.WriteCookie("IsFirstRun", "true");
            //Session.Mode = SessionStateMode.StateServer;
            
            if (Session["IsFirstRun"] == null)
            {
                Session["IsFirstRun"] = true;
            }
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
    }
}