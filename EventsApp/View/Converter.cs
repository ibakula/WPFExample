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
using EventsApp.Model;

namespace EventsApp.View
{
    class Converter
    {
        static public void ConvertXmlToObject(string url, BaseViewModel bvm) // https://channel9.msdn.com/Events/Build/2015/RSS / https://s.ch9.ms/Events/Build/2015/RSS
        {
            if (bvm == null)
                return;
            
            using (WebClient client = new WebClient())
            {
                try
                {
                    var xml = client.DownloadData(url);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(rss));
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(xml, 0, xml.Length);
                        stream.Position = 0;
                        rss feed = (rss)xmlSerializer.Deserialize(stream);
                        bvm.Feed = feed;
                    }
                }
#pragma warning disable 168
                catch (Exception e) // Exception could occur if document unreachable.
                {
                    // ToDo: Handle
                }
#pragma warning restore 168
            }
        }
    }
}
