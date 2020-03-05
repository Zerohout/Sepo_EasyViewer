// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;

	public partial class AddressEditingViewModel : Screen
	{
		public AddressEditingViewModel()
		{
			
		}

		public AddressEditingViewModel(EpAddress address, EpisodesEditorViewModel parent)
		{
			CurrentAddress = address;
			NewAddress = address.Address;
			AddressSnapshot = JsonConvert.SerializeObject(CurrentAddress);
			Parent = parent;
		}
	}
}
