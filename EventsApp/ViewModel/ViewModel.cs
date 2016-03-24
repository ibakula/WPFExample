using EventsApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using EventsApp.View;

namespace EventsApp.ViewModel
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class BaseViewModel : ObservableObject
    {
        protected rss _feed = null;
        public Page _page = null;

        public rss Feed
        {
            get
            {
                return _feed;
            }
            set
            {
                _feed = value;
                Items.Clear();
                foreach (var item in _feed.channel.item)
                    Items.Add(new ItemViewModel(item));
            }
        }

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();
    }

    public class HomeViewModel : BaseViewModel
    {
        protected int _selectionId = 0;

        public int Selection
        {
            get
            {
                return _selectionId;
            }
            set
            {
                _selectionId = value;
                RaisePropertyChangedEvent(nameof(Description));
            }
        }
        
        public string Description
        {
            get
            {
                if (!SelectionIsValid())
                    return "No description available.";
                else return Items.ElementAt(_selectionId).Description + "\n\nPublished on: " + Items.ElementAt(_selectionId).publishDate + "\nCreators: " + Items.ElementAt(_selectionId).Author;
            } 
        }

        public ICommand News
        {
            get
            {
                return new DelegateCommand(NavigateToNews);
            }
        }

        private void NavigateToNews()
        {
            if (_page != null)
            {
                ItemViewModel ivm = Items.ElementAt(_selectionId);
                ViewVideo viewVideo = new ViewVideo(ivm);
                _page.NavigationService.Navigate(viewVideo);
            }
        }

        protected bool SelectionIsValid()
        {
            return (Items.Count() > _selectionId);
        }
    }

    public class VideoViewModel : BaseViewModel
    {
        private bool _playedPreviously = false;
        private ItemViewModel _ivm = null;
        private TimeSpan _videoPosition = TimeSpan.FromSeconds(0);
        private List<string> _videoQuality = new List<string>();
        private List<Uri> _videoQualityUrl = new List<Uri>();
        private int _selectedQualityIndex =  0;

        public IEnumerable<string> AvailableQuality
        {
            get
            {
                if (_ivm == null)
                    return new string[3] { "High", "Medium", "Low" };

                return _videoQuality;
            }
        }

        public ItemViewModel ItemView
        {
            get
            {
                return _ivm;
            }
            set
            {
                _ivm = value;
                SetDefaultSelection();
                RaisePropertyChangedEvent(nameof(Selection));
                RaisePropertyChangedEvent(nameof(Description));
                RaisePropertyChangedEvent(nameof(AvailableQuality));
            }
        }

        public ICommand ButtonAction
        {
            get
            {
                return new DelegateCommand(DoAction);
            }
        }

        public string Description
        {
            get
            {
                if (_ivm == null)
                    return "No description available.";
                else return _ivm.Description + "\n\nPublished on: " + _ivm.publishDate + "\nCreators: " + _ivm.Author;
            }
        }

        public Uri Selection
        {
            get
            {
                if (_ivm == null)
                    return null;

                if (!_playedPreviously)
                    return _videoQualityUrl[0];

                if (_videoQualityUrl.Count() < _selectedQualityIndex + 1 || _videoQualityUrl[_selectedQualityIndex+1] == null)
                    SetDefaultSelection();

                return _videoQualityUrl[_selectedQualityIndex+1];
            }
        }

        public int SelectedQuality
        {
            get
            {
                return _selectedQualityIndex;
            }
            set
            {
                _selectedQualityIndex = value;
                RaisePropertyChangedEvent(nameof(Selection));
            }
        }

        public void SetDefaultSelection()
        {
            string[,] extension = new string[,] { { "_high.mp4", "High" }, { "_mid.mp4", "Medium" }, { ".mp4", "Low" } };
            _videoQuality.Clear();
            _videoQualityUrl.Clear();
            _selectedQualityIndex = 0;
            _videoQualityUrl.Add(new Uri(_ivm.Thumbnail[0].url));

            for (int u = 0; u < _ivm.Video.Length; ++u)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (_ivm.Video[u].url.Contains(extension[i, 0]))
                    {
                        if (_videoQuality.Contains(extension[i, 1]))
                            continue;

                        _videoQuality.Add(extension[i, 1]);
                        _videoQualityUrl.Add(new Uri(_ivm.Video[u].url));
                        break;
                    }
                }
            }
        }

        public void DoAction()
        {
            ViewVideo viewVideo = _page as ViewVideo;
            _playedPreviously = (!_playedPreviously);
            viewVideo.button.Content = (((string)viewVideo.button.Content)?.Contains("Pause") == true ? "Play" : "Pause");
            if (!_playedPreviously)
                _videoPosition = viewVideo.videoElement.Position;

            RaisePropertyChangedEvent(nameof(Selection));
            viewVideo.videoElement.Position = _videoPosition;
        }
    }
}
