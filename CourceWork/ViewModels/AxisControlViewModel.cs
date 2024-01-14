using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourceWork.Services;
using Serilog;

namespace CourceWork.MVVM.ViewModels
{
    public partial class AxisControlViewModel : BaseViewModel
    {
        [ObservableProperty]
        public int xCoordinate = 0;
        [ObservableProperty]
        public int yCoordinate = 0;
        [ObservableProperty]
        public int xShiftCoordinate = 0;
        [ObservableProperty]
        public int yShiftCoordinate = 0;
        [ObservableProperty]
        public int zShiftCoordinate = 0;
        private int zFromFocus = 0;

        private readonly MaxiGrafService _maxiGrafService;

        public AxisControlViewModel(MaxiGrafService maxiGrafService)
        {
            _maxiGrafService = maxiGrafService;
        }
        [RelayCommand]
        public void JoystickRight()
        {
            int newCoordX = XCoordinate + 1;
            _maxiGrafService.MoveToX(newCoordX);
            XCoordinate = newCoordX;
            Log.Information("Сдвиг по оси X вправо");
        }
        [RelayCommand]
        public void JoystickUp()
        {
            int newCoordY = YCoordinate + 1;
            _maxiGrafService.MoveToY(newCoordY);
            YCoordinate = newCoordY;
            Log.Information("Сдвиг по оси Y вверх");
        }
        [RelayCommand]
        public void JoystickDown()
        {
            int newCoordY = YCoordinate - 1;
            _maxiGrafService.MoveToY(newCoordY);
            YCoordinate = newCoordY;
            Log.Information("Сдвиг по оси Y вниз");
        }
        [RelayCommand]
        public void JoystickLeft()
        {
            int newCoordX = XCoordinate - 1;
            _maxiGrafService.MoveToX(newCoordX);
            XCoordinate = newCoordX;
            Log.Information("Сдвиг по оси X влево");
        }
        [RelayCommand]
        public void AcceptXY()
        {
            try
            {
                _maxiGrafService.MoveToX(XShiftCoordinate);
                _maxiGrafService.MoveToY(YShiftCoordinate);
                XCoordinate = XShiftCoordinate;
                YCoordinate = YShiftCoordinate;
            }
            catch (Exception ex)
            {
                Log.Error("Метод: " + ex.TargetSite);
                Log.Error(ex.Message);
            }
        }
        [RelayCommand]
        public void AcceptZ()
        {
            try
            {
                _maxiGrafService.ShiftZ(ZShiftCoordinate);
                zFromFocus += ZShiftCoordinate;
            }
            catch (Exception ex)
            {
                Log.Error("Метод: " + ex.TargetSite);
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
