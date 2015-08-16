using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Hosting;

namespace Ionic.Zip.Examples
{
    public class Manager
    {
        public Manager()
        {
        }

        public void Run(string[] pages)
        {
            bool cleanBin = false;
            MyAspNetHost host = null;
            string vdir = "/AspNetHost";
            string cwd = Directory.GetCurrentDirectory();
            try
            {
                if (!Directory.Exists("bin"))
                {
                    cleanBin = true;
                    CreateSymbolicLink("bin", ".", 1);
                }
                host = (MyAspNetHost)ApplicationHost.CreateApplicationHost(typeof(MyAspNetHost), vdir, cwd);
                string[] strArrays = pages;
                for (int i = 0; i < strArrays.Length; i++)
                {
                    host.ProcessRequest(strArrays[i]);
                }
            }
            finally
            {
                if (host != null)
                {
                    AppDomain.Unload(host.GetAppDomain());
                    if (cleanBin)
                    {
                        Directory.Delete("bin");
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            if ((args == null ? false : (int)args.Length != 0))
            {
                try
                {
                    (new Manager()).Run(args);
                }
                catch (Exception exception)
                {
                    Exception ex = exception;
                    Console.WriteLine("{0}: {1}\n{2}", ex.GetType().ToString(), ex.Message, ex.StackTrace);
                }
            }
            else
            {
                Console.WriteLine("Usage:  AspNetHost <aspx url> ...");
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateSymbolicLinkW", ExactSpelling = false)]
        public static extern int CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);
    }
}
