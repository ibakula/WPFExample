using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Events
{
    public class PropertyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
 
        private void OnPropertyChangeEvent(string link)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(link));
        }
    }

    public class BaseViewModel : PropertyObject
    {
        public string test;
    }
}
