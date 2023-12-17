using CommunityToolkit.Mvvm.ComponentModel;

namespace CourceWork.MVVM.ViewModels
{

    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => !IsBusy;
    }
}