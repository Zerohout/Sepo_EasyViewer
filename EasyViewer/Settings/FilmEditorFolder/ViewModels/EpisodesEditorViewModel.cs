namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System.Linq;
	using Caliburn.Micro;
    using Models.FilmModels;

    public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		public EpisodesEditorViewModel()
		{
			
		}
		public EpisodesEditorViewModel(Episode episode)
		{
			CurrentEpisode = episode;
			EEditingVM = new EpisodeEditingViewModel(episode, this);
			ActiveItem = EEditingVM;
			_addresses = new BindableCollection<EpAddress>(episode.Addresses);
			SelectedAddress = Addresses?.FirstOrDefault();
			NotifyOfPropertyChange(() => Addresses);
		}

		protected override void OnInitialize()
		{
			DisplayName = "Редактор эпизодов";
			
			base.OnInitialize();
		}
	}
}
