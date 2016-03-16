using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using EventsApp.ViewModel;

namespace EventsApp.View
{
    class Converter
    {
        static public void Convert(string hostUrl, string suffixUrl, BaseViewModel bvm) // https://channel9.msdn.com/Events/Build/2015/RSS
        {
            WebClient client = new WebClient();
            client.BaseAddress = hostUrl;
            byte[] data = client.DownloadData(suffixUrl);
            string tags = Encoding.ASCII.GetString(data);
        }
    }
}
