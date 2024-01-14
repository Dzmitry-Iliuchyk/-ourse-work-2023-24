using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourceWork.MVVM.Views;
using CourceWork.Services;
using CourceWork.ViewModels;
using Serilog;
using System.ComponentModel;

namespace CourceWork.MVVM.ViewModels
{
    public partial class HomePageViewModel : BaseViewModel
    {
        [ObservableProperty]
        public string path = "";
        [ObservableProperty]
        public string markingTime = "--:--";
        [ObservableProperty]
        public bool isStopButtonEnabled = true;
        [ObservableProperty]
        public bool isDetailsReadyButtonEnabled = true;
        [ObservableProperty]
        public bool isStartMarkButtonEnabled = true;
        [ObservableProperty]
        public bool isSelectFileButtonEnabled = true;


        private readonly MaxiGrafService _maxiGrafService;
        private readonly AppShellViewModel _appShellViewModel;
        

        public HomePageViewModel(MaxiGrafService maxiGrafService, AppShellViewModel appShellViewModel)
        {
            _maxiGrafService = maxiGrafService;
            _appShellViewModel = appShellViewModel;
        }

        [RelayCommand]
        public async Task LoadFileAsync()
        {
            IsBusy = true;
            var fileFromPlatform = await GetFileFromPlatformAsync();
            if (fileFromPlatform != null)
            {
                string pat ="";
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.ProgressChanged += (s, e) => {
                    Path = ($"Загрузка: {e.ProgressPercentage}%");
                };
                try
                {
                    pat = await _maxiGrafService.SelectFileProcess(fileFromPlatform, worker);
                }
                catch (Exception ex)
                {
                    Log.Error("Load file exception:"+ex.Message);
                }
                Path = pat;
                MarkingTime = _maxiGrafService.markingTime;
            }
            IsBusy = false;
        }

        [RelayCommand]
        public async Task StartMarkAsync()
        {
            _maxiGrafService.StartButton_Click();
            DisableButtons();
        }
        [RelayCommand]
        public async Task DetailReadyAsync()
        {
            _maxiGrafService.SetDetailsReady();
        }

        [RelayCommand]
        public async Task ExtraStopAsync()
        {
            _maxiGrafService.button_Stop_Click();
            EnableButtons();
        }
        private async Task<FileResult?> GetFileFromPlatformAsync()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.text", "public.my.le.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "text/plain", "application/octet-stream" } }, // MIME type
                        { DevicePlatform.WinUI, new[] { ".txt", ".le" } }, // file extension
                        { DevicePlatform.Tizen, new[] { ".txt", ".le" } },
                        { DevicePlatform.macOS, new[] { "public.text", "public.my.le.extension" } }, // UTType values
                    });
                var options = new PickOptions { FileTypes = customFileType, PickerTitle = "Select Le/txt file" };
                var result = await FilePicker.Default.PickAsync(options);
                PickOptions pickOptions = new();

                Path = result?.FileName??"не выбрано";
                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
            return null;
        }
        private void EnableButtons()
        {
            IsSelectFileButtonEnabled = true;
            IsStartMarkButtonEnabled = true;
            _appShellViewModel.IsEnabled = true;
        }
        private void DisableButtons()
        {
            IsSelectFileButtonEnabled = false;
            IsStartMarkButtonEnabled = false;
            _appShellViewModel.IsEnabled = false;
        }
    }

}
