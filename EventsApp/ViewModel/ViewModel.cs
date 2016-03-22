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
        private int _selectedQualityIndex = 0;
        private ItemViewModel _ivm = null;
        private TimeSpan _videoPosition = TimeSpan.FromSeconds(0);
        private List<string> _videoQuality = new List<string>();

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
                RaisePropertyChangedEvent("Selection");
                RaisePropertyChangedEvent("Description");
                RaisePropertyChangedEvent("AvailableQuality");
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
                    return new Uri(_ivm.Thumbnail[0].url);

                if (_ivm.Video.Count() < _selectedQualityIndex || _ivm.Video.ElementAt(_selectedQualityIndex).url == String.Empty)
                    SetDefaultSelection();

                return new Uri(_ivm.Video[_selectedQualityIndex].url);
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
                RaisePropertyChangedEvent("Selection");
            }
        }

        public void SetDefaultSelection()
        {
            string[,] extension = new string[,] { { "_high.mp4", "High" }, { "_mid.mp4", "Medium" }, { ".mp4", "Low" } };

            foreach (var video in _ivm.Video)
                for (int i = 0; i < 2; ++i)
                    if (video.url.Contains(extension[i, 0]))
                    {
                        if (i == 0 || i == 2)
                            _selectedQualityIndex = i;

                        _videoQuality.Add(extension[i, 1]);
                        break;
                    }

        }

        public void DoAction()
        {
            ViewVideo viewVideo = _page as ViewVideo;
            if (viewVideo.button.Content.ToString() == "Pause")
            {
                _videoPosition = viewVideo.videoElement.Position;
                viewVideo.videoElement.Source = new Uri(_ivm.Thumbnail[0].url);
                viewVideo.button.Content = "Play";
            }
            else
            {
                if (!_playedPreviously)
                    _playedPreviously = true;

                viewVideo.videoElement.Source = new Uri(_ivm.Video[_selectedQualityIndex].url);
                viewVideo.videoElement.Position = _videoPosition;
                viewVideo.button.Content = "Pause";
            }
        }
    }
}
