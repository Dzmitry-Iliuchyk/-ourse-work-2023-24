using CommunityToolkit.Maui;
using CourceWork.MVVM.ViewModels;
using System.Net;
namespace CourceWork.MVVM.Views;

public partial class ConnectPage : ContentPage
{
	public ConnectPage(ConnectPageViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
	}
	
}