using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourceWork.MVVM.Views;
using CourceWork.Services;
using Microsoft.Extensions.DependencyModel;
using Serilog;

namespace CourceWork.MVVM.ViewModels
{
    public partial class ConnectPageViewModel : BaseViewModel
    {
        private readonly MaxiGrafService _maxiGraf;

        [ObservableProperty]
        public string host = null!;
        [ObservableProperty]
        public int port;

        private readonly AppShell _appShell;

        public ConnectPageViewModel(MaxiGrafService maxiGraf, AppShell appShell)
        {
            _maxiGraf = maxiGraf;
            _appShell = appShell;
        }

        [RelayCommand]
        public async Task ConnectAsync()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                if (!ConvertIpAddress(Host, out IPAddress address))
                {
                    IsBusy = false;
                    return;
                }
                _maxiGraf.Host = address;
                _maxiGraf.Port = Port;
                await _maxiGraf.TryConnectAsync();
                Application.Current.MainPage = _appShell;
                Log.Error("Logger");
                IsBusy = false;
            }
        }
        
        private bool ConvertIpAddress(string host, out IPAddress address) 
        {
            if (IPAddress.TryParse(host,out address))
            {
                return true;
            }
            else
            {
                Shell.Current.DisplayAlert("Error!",$"{host} - это не Ip-адрес", "Ok");
                return false;
            }
        }
    }
}
