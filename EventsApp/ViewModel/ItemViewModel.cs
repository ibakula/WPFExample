﻿using EventsApp.Model;
using System;

namespace EventsApp.ViewModel
{
    public class ItemViewModel : ObservableObject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string publishDate { get; set; }
        public thumbnail[] Thumbnail { get; set; }
        public groupContent[] Video { get; set; }
        public Uri Thumb { get { return new Uri(Thumbnail[0].url); } }

        public ItemViewModel()
        {
        }

        public ItemViewModel(rssChannelItem item)
        {
            Title = item.title;
            Description = item.summary;
            Thumbnail = item.thumbnail;
            Video = item.group;
            Author = item.creator;
            publishDate = item.pubDate;
        }
    }
}
