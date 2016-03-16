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
    public class ListObject
    {
        public ObservableCollection<string> _descriptionList = new ObservableCollection<string>();
        public ObservableCollection<string> _linkList = new ObservableCollection<string>();
        public ObservableCollection<string> _newsList = new ObservableCollection<string>();
    }

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
        private ListObject _list;
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
                OnPropertyChangeEvent("Selection");
            }
        }

        public IEnumerable<string> GetNewsList
        {
            get
            {
                return _list._newsList;
            }
        }

        public string GetDescription
        {
            get
            {
                if (_list._descriptionList.Count >= _selectionId || _list._descriptionList[_selectionId] == null)
                    return "No description available";
                else return _list._descriptionList[_selectionId];
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
