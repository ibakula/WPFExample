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
        protected Page _page = null;

        public virtual Page page { get { return _page; } set { _page = value; } }

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

                RaisePropertyChangedEvent(nameof(Items));
            }
        }

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();
    }

    public class HomeViewModel : BaseViewModel
    {
        protected int _selectionId = 0;

        public override Page page { get { return _page; } set { _page = value; RaisePropertyChangedEvent(nameof(Description)); } }

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

        protected bool SelectionIsValid()
        {
            return (Items.Count() > _selectionId);
        }
    }

    public class PreviewViewModel : BaseViewModel
    {
        private ItemViewModel _ivm = null;
        private List<Uri> _videoQualityUrl = new List<Uri>();
        private int _selectedQualityIndex =  0;

        public List<string> VideoQuality { get; } = new List<string>();

        public ItemViewModel ItemView
        {
            get
            {
                return _ivm;
            }
            set
            {
                _ivm = value;
                LoadSelections();
                RaisePropertyChangedEvent(nameof(SelectedQuality));
                RaisePropertyChangedEvent(nameof(Description));
                RaisePropertyChangedEvent(nameof(VideoQuality));
            }
        }

        public ICommand Play
        {
            get
            {
                return new DelegateCommand(NavigateToPlayPage);
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

        public void NavigateToPlayPage()
        {
            Video video = new Video(Selection);
            _page.NavigationService.Navigate(video);
        }

        private Uri Selection
        {
            get
            {
                if (_ivm == null)
                    return null;

                if (_selectedQualityIndex < 0 || _videoQualityUrl.Count() < _selectedQualityIndex || _videoQualityUrl[_selectedQualityIndex] == null)
                    LoadSelections();

                return _videoQualityUrl[_selectedQualityIndex];
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

        public void LoadSelections()
        {
            string[,] extension = new string[,] { { "_high.mp4", "High" }, { "_mid.mp4", "Medium" }, { ".mp4", "Low" } };
            VideoQuality.Clear();
            _videoQualityUrl.Clear();
            _selectedQualityIndex = 0;

            for (int u = 0; u < _ivm.Video.Length; ++u)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (_ivm.Video[u].url.Contains(extension[i, 0]))
                    {
                        if (VideoQuality.Contains(extension[i, 1]))
                            continue;

                        VideoQuality.Add(extension[i, 1]);
                        _videoQualityUrl.Add(new Uri(_ivm.Video[u].url));
                        break;
                    }
                }
            }
        }
    }

    public class VideoViewModel : BaseViewModel
    {
        private Uri _video = null;
        public Uri video { get { return _video; } set { _video = value; RaisePropertyChangedEvent(nameof(video)); } }

        public ICommand ButtonAction
        {
            get
            {
                return new DelegateCommand(DoAction);
            }
        } 

        public ICommand ButtonStop
        {
            get
            {
                return new DelegateCommand(StopVideo);
            }
        }

        public void DoAction()
        {
            Video viewVideo = _page as Video;
            bool isPlaying = (viewVideo.actionButton.Content as string).Contains("Pause");
            viewVideo.actionButton.Content = (isPlaying ? "Play" : "Pause");

            if (!isPlaying)
                viewVideo.videoElement.Play();
            else viewVideo.videoElement.Pause();

            viewVideo.stopButton.IsEnabled = true;
        }

        public void StopVideo()
        {
            Video viewVideo = _page as Video;
            viewVideo.videoElement.Stop();
            viewVideo.actionButton.Content = "Play";
            viewVideo.stopButton.IsEnabled = false;
        }
    }
}
