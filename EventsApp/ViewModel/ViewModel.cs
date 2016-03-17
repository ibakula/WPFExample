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
        private int _selectionId = 0;
        public Page _page;

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

        public SyndicationFeed GetFeed
        {
            get
            {
                return _feed;
            }
            set
            {
                _feed = value;
            }
        }

        public string GetDescription
        {
            get
            {
                if (_feed == null || _feed.Items.Count() >= _selectionId || _feed.Items.ElementAt(_selectionId) == null)
                    return "No description available";
                else return _feed.Items.ElementAt(_selectionId).Title.Text;
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

        public ICommand ShowNews
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
                ViewVideo viewVideo = new ViewVideo();
                _page.NavigationService.Navigate(viewVideo);
            }
        }
    }
}
