using BugTrackLite.ViewModels;

namespace BugTrackLite.Views;

public partial class MorePage : ContentPage
{
    public MorePage(MoreViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
