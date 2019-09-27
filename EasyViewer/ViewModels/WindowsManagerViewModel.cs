namespace EasyViewer.ViewModels
{
    using Caliburn.Micro;

    public class WindowsManagerViewModel : Conductor<Screen>.Collection.OneActive
	{
		public WindowsManagerViewModel(Screen activeItem) { ActiveItem = activeItem; }

		public WindowsManagerViewModel()
		{
			
		}
	}
}
