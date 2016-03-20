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
using System.ServiceModel.Syndication;

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
        private SyndicationFeed _feed = null;
        public Page _page = null;
        protected int _selectionId = 0;

        public string GetDescription
        {
            get
            {
                if (_feed == null || !SelectionIsValid())
                    return "No description available";
                else return _feed.Items.ElementAt(_selectionId).Summary.Text;
            }
        }

        public SyndicationFeed GetFeed
        {
            get
            {
                return _feed;
            }
            set
            {
                _feed = value;
                OnPropertyChangeEvent("GetTitlesList");
                OnPropertyChangeEvent("GetDescription");
            }
        }

        public IEnumerable<string> GetTitlesList
        {
            get
            {
                ObservableCollection<string> titlesList = new ObservableCollection<string>();
                if (_feed != null)
                {
                    foreach (var item in _feed.Items)
                    {
                        titlesList.Add(item.Title.Text);
                    }
                }

                return titlesList;
            }
        }

        protected bool SelectionIsValid()
        {
            return (_feed.Items.Count() < _selectionId || _feed.Items.ElementAt(_selectionId) != null);
        }
    }

    public class HomeViewModel : BaseViewModel
    {
        public int Selection
        {
            get
            {
                return _selectionId;
            }
            set
            {
                _selectionId = value;
                OnPropertyChangeEvent("GetDescription");
            }
        }

        public ICommand ShowNews
        {
            get
            {
                return new DelegateCommand(NavigateToNews);
            }
        }

        private void NavigateToNews()
        {
            if (_page != null && SelectionIsValid())
            {
                ViewVideo viewVideo = new ViewVideo();
                _page.NavigationService.Navigate(viewVideo);
            }
        }
    }

    public class VideoViewModel : BaseViewModel
    {

    }
}
