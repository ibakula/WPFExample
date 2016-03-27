using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventsApp.ViewModel;

namespace EventsApp.View
{
    /// <summary>
    /// Interaction logic for ViewVideo.xaml
    /// </summary>
    public partial class ViewVideo : Page
    {
        public ViewVideo()
        {
            InitializeComponent();
            this.InitData();
        }

        public ViewVideo(ItemViewModel ivm) : this()
        {
            VideoViewModel vvm = DataContext as VideoViewModel;
            vvm.ItemView = ivm;
            actionButton.IsEnabled = true;
        }
    }
}
