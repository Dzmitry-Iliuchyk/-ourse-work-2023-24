using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourceWork.Services;
using Serilog;
using System.Globalization;
using System.Net;

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

                if (!ConvertIpAddress2(Host, out IPAddress? address))
                {
                    IsBusy = false;
                    return;
                }
                _maxiGraf.Host = address;
                _maxiGraf.Port = Port;
                if (!(await _maxiGraf.TryConnectAsync()))
                {
                    IsBusy = false;
                    await App.Current.MainPage.DisplayAlert("Error!", $"Не удалось подключиться, попробуйте еще раз.", "Ok");
                    return;
                }
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
                App.Current.MainPage.DisplayAlert("Error!",$"{host} - это не Ip-адрес", "Ok");
                return false;
            }
        }
        private bool ConvertIpAddress2(string host, out IPAddress? address) 
        {
            try
            {
                string[] parts = host.Split('.');
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] = byte.Parse(parts[i], NumberStyles.None);
                }
                address = new IPAddress(bytes);
            }
            catch (Exception)
            {
                Log.Error($"Failed to convert {host}");
                address = null;
                App.Current.MainPage.DisplayAlert("Error!", $"{host} - это не Ip-адрес", "Ok");
                return false;
            }
            return true;
        }
    }
}
