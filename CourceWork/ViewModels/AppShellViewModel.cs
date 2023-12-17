using CommunityToolkit.Mvvm.ComponentModel;
using CourceWork.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourceWork.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {
        [ObservableProperty]
        public bool isEnabled = true;
    }
}
