using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EventsApp.ViewModel;

namespace EventsApp.View
{
    public static class PageExtension
    {
        static public void InitData(this Page page, Action _action = null)
        {
            BaseViewModel bvm = page.DataContext as BaseViewModel;
            if (bvm != null)
            {
                bvm.page = page;
                _action?.Invoke();
            }
        }
    }
}
