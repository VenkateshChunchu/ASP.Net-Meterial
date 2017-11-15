using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ASPNetDemoWebApplication
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
             
            Application["TotalApplications"] = 0;
            Application["TotalUserSessions"] = 0;

            Application["TotalApplications"] = (int)Application["TotalApplications"] + 1;
        }
        protected void Application_End(object sender, EventArgs e)
        {

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                Logger.Log(ex);
                Server.ClearError();
                Response.Redirect("~/Error.aspx");
            }
        }
        
        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["TotalUserSessions"] = (int)Application["TotalUserSessions"] + 1;
            Application.UnLock();
        }
        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["TotalUserSessions"] = (int)Application["TotalUserSessions"] - 1;
            Application.UnLock();
        }
    }
}