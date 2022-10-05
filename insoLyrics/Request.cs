using System.Net;

namespace insoLyrics
{
    class Request
    {
        public static HttpWebRequest Create(string url)
        {
            var wr = (HttpWebRequest) WebRequest.Create(url);
            wr.ServicePoint.Expect100Continue = false;
            return wr;
        }
    }
}
