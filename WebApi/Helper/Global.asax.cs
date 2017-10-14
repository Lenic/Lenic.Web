using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.SessionState;

namespace Helper
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            TextWriter tw = new StringWriter();
            HttpWorkerRequest wr = new SimpleWorkerRequest("default.aspx", "friendId=1300000000", tw);
            HttpContext.Current = new HttpContext(wr);

            var a = Toolkit.GetTypeURL<int>();
            var b = Toolkit.GetTypeURL<int?>();
            var c = Toolkit.GetTypeURL<string>();
            var d = Toolkit.GetTypeURL<int[]>();
            var f = Toolkit.GetTypeURL<MyClass[]>();
            var g = Toolkit.GetTypeURL<List<string>>();
            var h = Toolkit.GetTypeURL<List<IEnumerable<MyClass>>>();
            var i = Toolkit.GetTypeURL<List<KeyValuePair<MyClass, Global>>>();
            var j = Toolkit.GetTypeURL<List<KeyValuePair<MyClass, MyStruct>>>();

            var a1 = Toolkit.GetHtmlString(a);
            var b1 = Toolkit.GetHtmlString(b);
            var c1 = Toolkit.GetHtmlString(c);
            var d1 = Toolkit.GetHtmlString(d);
            var f1 = Toolkit.GetHtmlString(f);
            var g1 = Toolkit.GetHtmlString(g);
            var h1 = Toolkit.GetHtmlString(h);
            var i1 = Toolkit.GetHtmlString(i);
            var j1 = Toolkit.GetHtmlString(j);
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
    }

    public class MyClass
    {
    }

    public struct MyStruct
    {
    }
}