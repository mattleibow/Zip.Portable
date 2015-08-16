using System.IO;
using System.Web.Hosting;

namespace Ionic.Zip.Examples
{
    public class BinaryCapableRequest : SimpleWorkerRequest
    {
        private Stream outStream;

        public BinaryCapableRequest(string page, string query, Stream output)
            : base(page, query, null)
        {
            outStream = output;
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            outStream.Write(data, 0, length);
        }
    }
}
