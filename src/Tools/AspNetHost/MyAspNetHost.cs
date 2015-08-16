using System;
using System.Threading;
using System.Web;

namespace Ionic.Zip.Examples
{
    public class MyAspNetHost : MarshalByRefObject
    {
        public MyAspNetHost()
        {
        }

        public AppDomain GetAppDomain()
        {
            return Thread.GetDomain();
        }

        public void ProcessRequest(string url)
        {
            var components = url.Split('?');
            var page = components[0];
            string str;
            if (components.Length > 1)
            {
                str = components[1];
            }
            else
            {
                str = null;
            }
            HttpRuntime.ProcessRequest(new BinaryCapableRequest(page, str, Console.OpenStandardOutput()));
        }
    }
}
