using CourceWork.MVVM.Views;
using Microsoft.Maui.Platform;

namespace CourceWork
{
    public partial class App : Application
    {
        public App(ConnectPage connectPage)
        {
            InitializeComponent();

            MainPage = connectPage;
        }
    }
}
