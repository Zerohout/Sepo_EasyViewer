namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;

	public partial class EpisodeEditingViewModel : Screen
	{
		public EpisodeEditingViewModel()
		{
			
		}

		public EpisodeEditingViewModel(Episode episode, EpisodesEditorViewModel parent)
		{
			CurrentEpisode = episode;
			EpisodeSnapshot = JsonConvert.SerializeObject(CurrentEpisode);
			Parent = parent;
		}
	}
}
