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
            string xmlData = String.Empty;
            using (WebClient client = new WebClient())
            {
                client.BaseAddress = hostUrl;
                byte[] data = client.DownloadData(suffixUrl);
                xmlData = Encoding.ASCII.GetString(data);
            }
            using (StreamWriter sw = new StreamWriter(File.Open("rss.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)))
            {
                sw.Write(xmlData);
            }
            using (XmlReader xr = XmlReader.Create("rss.xml"))
            {
                xr.Read();
                SyndicationFeed syn = SyndicationFeed.Load(xr);
                bvm.GetFeed = syn;
            }
        }
    }
}
