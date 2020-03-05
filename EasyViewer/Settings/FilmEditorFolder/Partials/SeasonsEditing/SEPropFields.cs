// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using static Helpers.GlobalMethods;

	public partial class SeasonsEditingViewModel : Screen
	{
		private Season _currentSeason;
		private Episode _selectedEpisode;
		private string _seasonDescription;
		private int? _addingEpisodeValue = 1;

		#region Свойства сезонов

		/// <summary>
		/// Текущий сезон
		/// </summary>
		public Season CurrentSeason
		{
			get => _currentSeason;
			set
			{
				_currentSeason = value;
				NotifyOfPropertyChange(() => CurrentSeason);
			}
		}

		/// <summary>
		/// Экземпляр сезона для отслеживания изменений
		/// </summary>
		public string SeasonDescription
		{
			get => _seasonDescription;
			set
			{
				_seasonDescription = value;
				NotifyOfPropertyChange(() => SeasonDescription);
			}
		}

		#endregion

		#region Свойства эпизодов

		/// <summary>
		/// Количество создаваемых эпизодов по умолчанию за раз
		/// </summary>
		public int? AddingEpisodeValue
		{
			get => _addingEpisodeValue;
			set
			{
				if (value == null ||
				    value < 1)
				{
					value = 1;
				}

				if (value > 99)
				{
					value = 99;
				}

				_addingEpisodeValue = value;
				NotifyOfPropertyChange(() => AddingEpisodeValue);
			}
		}

		/// <summary>
		/// Список эпизодов текущего сезона
		/// </summary>
		public BindableCollection<Episode> Episodes => new BindableCollection<Episode>(CurrentSeason.GetEpisodes());

		/// <summary>
		/// Выбранный эпизод
		/// </summary>
		public Episode SelectedEpisode
		{
			get => _selectedEpisode;
			set
			{
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				NotifyEditingButtons();
			}
		}

		#endregion

		#region Общие свойства

		/// <summary>
		/// Модель представления селектора редактора
		/// </summary>
		private EditorSelectorViewModel ESVM => Parent as EditorSelectorViewModel;

		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges =>
			string.IsNullOrWhiteSpace(SeasonDescription) is false && CurrentSeason.Description != SeasonDescription;

		#endregion

	}
}