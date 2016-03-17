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
        static public void Convert(string hostUrl, string suffixUrl, BaseViewModel bvm) // https://channel9.msdn.com/Events/Build/2015/RSS / https://s.ch9.ms/Events/Build/2015/RSS
        {
            byte[] data;
            using (WebClient client = new WebClient())
            {
                client.BaseAddress = hostUrl;
                data = client.DownloadData(suffixUrl);
            }
            using (BinaryWriter bw = new BinaryWriter(File.Create("rss.xml"), Encoding.ASCII))
            {
                bw.Write(data);
                using (XmlReader xr = XmlReader.Create(bw.BaseStream))
                {
                    SyndicationFeed syn = SyndicationFeed.Load(xr);
                    bvm.GetFeed = syn;
                }
            }
        }
    }
}
