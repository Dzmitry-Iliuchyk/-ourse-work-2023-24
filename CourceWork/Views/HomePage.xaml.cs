using CourceWork.MVVM.ViewModels;

namespace CourceWork.MVVM.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomePageViewModel homePageViewModel)
	{
		InitializeComponent();
		BindingContext = homePageViewModel;
	}
}