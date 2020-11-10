// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Helpers;
    using LibVLCSharp.Shared;
    using Models.FilmModels;
	using Newtonsoft.Json;

	public partial class AddressEditingViewModel : Screen
	{
		public AddressEditingViewModel()
		{
			
		}

		public AddressEditingViewModel(AddressInfo addressInfo, EpisodesEditorViewModel parent)
		{
			_libVlc = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVlc) {EnableMouseInput = false};
            MediaPlayer.TimeChanged += MediaPlayerOnTimeChanged;
			CurrentAddressInfo = addressInfo;
			NewAddress = addressInfo.Link;
			AddressSnapshot = JsonConvert.SerializeObject(CurrentAddressInfo);
			Parent = parent;
			Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
            NotifyOfPropertyChange(()=> CanPasteFilmEndTime);
            NotifyOfPropertyChange(()=> MediaPlayer);
            NotifyOfPropertyChange(() => CanAddFirstJumper);
            NotifyOfPropertyChange(() => CanAddVolumeJumper);
            SystemVariables.IsEditDefaultAddressInfo = false;
        }

    }
}
