// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using  static Helpers.GlobalMethods;

	public partial class EpisodeEditingViewModel : Screen
	{
		private Episode _currentEpisode;
		private string _episodeSnapshot;

		public Episode CurrentEpisode
		{
			get => _currentEpisode;
			set
			{
				_currentEpisode = value;
				NotifyOfPropertyChange(() => CurrentEpisode);
			}
		}

		/// <summary>
		/// Экземпляр эпизода для отслеживания изменений
		/// </summary>
		public string EpisodeSnapshot
		{
			get => _episodeSnapshot;
			set
			{
				_episodeSnapshot = value;
				NotifyOfPropertyChange(() => EpisodeSnapshot);
			}
		}
		/// <summary>
		/// Родительская модель представления
		/// </summary>
		public EpisodesEditorViewModel EEVM => Parent as EpisodesEditorViewModel;

		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges => CurrentEpisode != null &&
		                          IsEquals(CurrentEpisode, JsonConvert.DeserializeObject(EpisodeSnapshot)) is false;

	}
}