using CourceWork.ViewModels;

namespace CourceWork
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            BindingContext = appShellViewModel;
        }
    }
}
