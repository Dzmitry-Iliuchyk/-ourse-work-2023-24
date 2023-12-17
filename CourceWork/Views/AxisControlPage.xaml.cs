using CourceWork.MVVM.ViewModels;

namespace CourceWork.MVVM.Views;

public partial class AxisControlPage : ContentPage
{
	public AxisControlPage(AxisControlViewModel axisControlViewModel)
	{
		InitializeComponent();
		BindingContext = axisControlViewModel;
		
	}
}