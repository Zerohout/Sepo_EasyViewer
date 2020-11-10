// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System;
	using System.Linq;
	using System.Windows;
    using System.Windows.Markup;
    using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using static Helpers.GlobalMethods;
	using static Helpers.SystemVariables;

	public partial class FilmsEditingViewModel : Screen
	{
		private Film _currentFilm;
		private string _filmSnapshot;
		private Season _selectedSeason;
		private int? _addingSeasonValue = 1;
        private BindableCollection<Season> _seasons;
        private BindableCollection<Season> _selectedSeasons = new BindableCollection<Season>();

		#region Свойства фильмов

		/// <summary>
		/// Список типов фильма
		/// </summary>
		public BindableCollection<FilmType> FilmTypes =>
			new BindableCollection<FilmType>(Enum.GetValues(typeof(FilmType)).Cast<FilmType>());

        public Film OriginalFilm { get; set; }

		/// <summary>
		/// Текущий фильм
		/// </summary>
		public Film CurrentFilm
		{
			get => _currentFilm;
			set
			{
				_currentFilm = value;
				NotifyOfPropertyChange(() => CurrentFilm);
			}
        }

		///// <summary>
		///// Экземпляр фильма для отслеживания изменений
		///// </summary>

		//public string FilmSnapshot
		//{
		//	get => _filmSnapshot;
		//	set
		//	{
		//		_filmSnapshot = value;
		//		NotifyOfPropertyChange(() => FilmSnapshot);
		//	}
		//}

		#endregion

		#region Свойства сезонов

        /// <summary>
        /// Список сезонов выбранного фильма
        /// </summary>
        public BindableCollection<Season> Seasons
        {
            get => _seasons;
            set
            {
				_seasons = new BindableCollection<Season>(value.OrderBy(s => s.Number));
				NotifyOfPropertyChange(()=> Seasons);
            }
        }

		/// <summary>
		/// Выбранный сезон
		/// </summary>
		public Season SelectedSeason
		{
			get => _selectedSeason;
			set
			{
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
				NotifyOfPropertyChange(() => CanCancelSelection);
				
			}
		}
        public BindableCollection<Season> SelectedSeasons
        {
            get => _selectedSeasons;
            set
            {
                _selectedSeasons = value;
				NotifyOfPropertyChange(() => SelectedSeasons);
				NotifyOfPropertyChange(() => CanDeleteSelectedSeasons);
				NotifyOfPropertyChange(() => CanModifySeason);
                NotifyOfPropertyChange(() => CanSelectSeason);
                NotifyOfPropertyChange(() => CanCancelSelection);
            }
        }

		#endregion

		#region Общие свойства

		/// <summary>
		/// Модель представления селектора редактора
		/// </summary>
		private EditorSelectorViewModel ESVM => Parent as EditorSelectorViewModel;


		public int? AddingSeasonValue
		{
			get => _addingSeasonValue;
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

				_addingSeasonValue = value;
				NotifyOfPropertyChange(() => AddingSeasonValue);
			}
		}


		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges => IsEquals(CurrentFilm, OriginalFilm) is false;

		/// <summary>
		/// Свойство Visibility контролов создания
		/// </summary>
		public Visibility CreateFilmVisibility => CurrentFilm.Name == NewFilmName
			? Visibility.Visible
			: Visibility.Collapsed;

		/// <summary>
		/// Свойство Visibility контролов редактирования
		/// </summary>
		public Visibility SaveChangesVisibility => CurrentFilm.Name != NewFilmName
			? Visibility.Visible
			: Visibility.Collapsed;

		#endregion

	}
}
