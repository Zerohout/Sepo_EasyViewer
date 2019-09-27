// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;
    using static Helpers.SystemVariables;

    public partial class EditorSelectorViewModel : Conductor<Screen>.Collection.OneActive
    {
        private FilmType? _selectedFilmType;
        private Film _selectedFilm;
        private Season _selectedSeason;
        private Episode _selectedEpisode;

        #region Типы фильмов

        /// <summary>
        /// Список типов фильма
        /// </summary>
        public BindableCollection<FilmType> FilmTypes =>
            new BindableCollection<FilmType>(Enum.GetValues(typeof(FilmType)).Cast<FilmType>());

        /// <summary>
        /// Выбранный тип фильма
        /// </summary>
        public FilmType? SelectedFilmType
        {
            get => _selectedFilmType;
            set
            {
                _selectedFilmType = value;
                NotifyOfPropertyChange(() => SelectedFilmType);
                SetFilms(value);
            }
        }

        #endregion

        #region Фильмы

        /// <summary>
        /// Модель представления фильма
        /// </summary>
        private FilmsEditingViewModel FEVM { get; set; }

        /// <summary>
        /// Список фильмов
        /// </summary>
        public BindableCollection<Film> Films { get; set; } = new BindableCollection<Film>();

        /// <summary>
        /// Выбранный фильм
        /// </summary>
        public Film SelectedFilm
        {
            get => _selectedFilm;
            set
            {
                _selectedFilm = value;
                NotifyOfPropertyChange(() => SelectedFilm);
                SetSeasons(value);
                SetFilmVM(value);
            }
        }

        /// <summary>
        /// Свойство Visibility фильмов
        /// </summary>
        public Visibility FilmsVisibility => SelectedFilmType != null
            ? Visibility.Visible
            : Visibility.Hidden;

        #endregion

        #region Сезоны

        /// <summary>
        /// Модель представления сезона
        /// </summary>
        private SeasonsEditingViewModel SEVM { get; set; }

        /// <summary>
        /// Список сезонов выбранного фильма
        /// </summary>
        public BindableCollection<Season> Seasons { get; set; } = new BindableCollection<Season>();

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
                SetEpisodes(value);

                if (FEVM == null) return;
                SetSeasonVM(value);
            }
        }

        /// <summary>
        /// Свойство Visibility сезонов
        /// </summary>
        public Visibility SeasonsVisibility => SelectedFilm != null && SelectedFilm.Name != NewFilmName
            ? Visibility.Visible
            : Visibility.Hidden;

        #endregion

        #region Эпизоды

        /// <summary>
        /// Модель представления эпизода
        /// </summary>
        private EpisodesEditingViewModel EEVM { get; set; }

        /// <summary>
        /// Список эпизодов выбранного сезона
        /// </summary>
        public BindableCollection<Episode> Episodes { get; set; } = new BindableCollection<Episode>();

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
                NotifyOfPropertyChange(() => CanCancelEpisodeSelection);

                if (SEVM == null) return;
                SetEpisodeVM(value);
            }
        }

        /// <summary>
        /// Свойство Visibility эпизодов
        /// </summary>
        public Visibility EpisodesVisibility => SelectedSeason != null
            ? Visibility.Visible
            : Visibility.Hidden;

        #endregion

    }
}
