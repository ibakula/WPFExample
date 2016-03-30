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
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : Page
    {
        public Video()
        {
            InitializeComponent();
            this.InitData();
        }

        public Video(Uri video):this()
        {
            VideoViewModel viewModel = DataContext as VideoViewModel;
            viewModel.video = video;
            actionButton.IsEnabled = true;
        }
    }
}
