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
using System.Xml.Linq;

namespace EventsApp.View
{
    class Converter
    {
        static public void Convert(string hostUrl, string suffixUrl, BaseViewModel bvm) // https://channel9.msdn.com/Events/Build/2015/RSS / https://s.ch9.ms/Events/Build/2015/RSS
        {
            if (bvm == null)
                return;

            byte[] data = null;
            EventsFeed feed = new EventsFeed();
            XDocument xD = XDocument.Load(hostUrl + "/" + suffixUrl);
            XNamespace[] nS = { "http://www.itunes.com/dtds/podcast-1.0.dtd", "http://purl.org/dc/elements/1.1/", "http://search.yahoo.com/mrss/" }; // itunes, dc, media  
            foreach (var item in xD.Descendants("item"))
            {
                FeedItem feedItem = new FeedItem();
                feedItem.title = item.Element("title").Value;
                feedItem.summary = item.Element(nS[0] + "summary").Value;
                feedItem.comments = item.Element("comments").Value;
                feedItem.creator = item.Element(nS[1] + "creator").Value;
                feedItem.pubDate = item.Element("pubDate").Value;
                foreach (var category in item.Elements("category"))
                    feedItem.category.Add(category.Value);
                foreach (var thumbnail in item.Elements(nS[2] + "thumbnail"))
                {
                    Thumbnail thumbnail = new Thumbnail();
                    feedItem.thumbnail.Add()
                }
            }
        }
    }
}
