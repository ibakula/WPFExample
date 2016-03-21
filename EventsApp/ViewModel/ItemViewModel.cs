using System.ServiceModel.Syndication;

namespace EventsApp.ViewModel
{
    public class ItemViewModel : ObservableObject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string VideoUrl { get; set; }

        public ItemViewModel()
        {
        }

        public ItemViewModel(SyndicationItem item)
        {
            Title = item.Title.Text;
            Description = item.Summary.Text;
        }
    }
}
