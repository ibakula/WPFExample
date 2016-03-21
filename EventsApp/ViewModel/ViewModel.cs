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

        protected void OnPropertyChangeEvent(string name)
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
                OnPropertyChangeEvent(nameof(Description));
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
        private int _selectedUrlId = 0;
        private ItemViewModel _ivm = null;
        private TimeSpan _videoPosition = TimeSpan.FromSeconds(0);

        public ItemViewModel ItemView
        {
            get
            {
                return _ivm;
            }
            set
            {
                _ivm = value;
                OnPropertyChangeEvent("Selection");
            }
        }

        public ICommand ButtonAction
        {
            get
            {
                return new DelegateCommand(DoAction);
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

                if (_ivm.Video.Count() < _selectedUrlId || _ivm.Video.ElementAt(_selectedUrlId).url == String.Empty)
                    SetDefaultSelection();

                return new Uri(_ivm.Video.ElementAt(_selectedUrlId).url);
            }
        }

        public void SetDefaultSelection()
        {
            for (; _selectedUrlId < _ivm.Video.Length; ++_selectedUrlId)
                if (_ivm.Video[_selectedUrlId].url.Contains("_high.mp4") || _ivm.Video[_selectedUrlId].url.Contains(".mp4"))
                    break;
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
                {
                    SetDefaultSelection();
                    _playedPreviously = true;
                }

                viewVideo.videoElement.Source = new Uri(_ivm.Video[_selectedUrlId].url);
                viewVideo.videoElement.Position = _videoPosition;
                viewVideo.button.Content = "Pause";
            }
        }
    }
}
