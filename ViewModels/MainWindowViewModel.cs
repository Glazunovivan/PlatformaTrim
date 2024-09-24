using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformaTrim.ViewModels
{
    internal class MainWindowViewModel
    {
        public object CurrentControl { get; set; }

        public MainWindowViewModel()
        {
            CurrentControl = new UserDataViewModel();
        }
    }
}
