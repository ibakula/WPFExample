using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Serialization;
using EventsApp.ViewModel;
using System.ServiceModel.Syndication;
using System.IO;
using System.Xml;

namespace EventsApp.View
{
    class Converter
    {
        /*
            ToDo: Add Action in args so we could get the 
            last element outcome 
            for each page and rather 
            use XML Serialization, as in 
            more convenient and extensible?
        */
        static public void Convert(string hostUrl, string suffixUrl, BaseViewModel bvm) // https://channel9.msdn.com/Events/Build/2015/RSS / https://s.ch9.ms/Events/Build/2015/RSS
        {
            if (bvm == null)
                return;

            byte[] data = null;
            using (WebClient client = new WebClient())
            {
                client.BaseAddress = hostUrl;
                try
                {
                    data = client.DownloadData(suffixUrl);
                }
#pragma warning disable 168
                catch (Exception e) // Exception could occur if document unreachable.
                {
                    // ToDo: Handle
                }
#pragma warning restore 168
            }
            if (data == null) // Failed to get?
                return;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                using (XmlReader xr = XmlReader.Create(ms))
                {
                    xr.Read();
                    SyndicationFeed syn = SyndicationFeed.Load(xr);
                    bvm.GetFeed = syn;
                }
            }
        }
    }
}
