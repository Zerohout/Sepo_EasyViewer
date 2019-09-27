// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;
    using FilmEditorFolder.ViewModels;
    using Models.FilmModels;


    public partial class ViewingsSettingsViewModel : Screen, ISettingsViewModel
    {
        private BindableCollection<Film> _films = new BindableCollection<Film>();
        private BindableCollection<Season> _seasons = new BindableCollection<Season>();
        private BindableCollection<Episode> _episodes = new BindableCollection<Episode>();


        private bool _isFilmListEnable;
        private bool _isSeasonListEnable;
        private bool _isEpisodeListEnable;

        private Film _selectedFilm;
        private Season _selectedSeason;
        private Episode _selectedEpisode;

        private (int FilmId, int SeasonId, int EpisodeId, int VoiceOverId) IdList = (0, 0, 0, 0);


        #region Flags

        public bool IsDesignTime { get; set; }

        #endregion


        #region Lists content

        /// <summary>
        /// Список фильмов
        /// </summary>
        public BindableCollection<Film> Films
        {
            get => _films;
            set
            {
                _films = value;
                NotifyOfPropertyChange(() => Films);
            }
        }

        /// <summary>
        /// список сезонов выбранного фильма
        /// </summary>
        public BindableCollection<Season> Seasons
        {
            get => _seasons;
            set
            {
                _seasons = value;
                NotifyOfPropertyChange(() => Seasons);
            }
        }

        /// <summary>
        /// Список эпизодов выбранного сезона
        /// </summary>
        public BindableCollection<Episode> Episodes
        {
            get => _episodes;
            set
            {
                _episodes = value;
                EpisodeIndexes.EndIndex = value?.Count - 1 ?? -1;
                NotifyOfPropertyChange(() => Episodes);
            }
        }

        /// <summary>
        /// Выбранный фильм
        /// </summary>
        public Film SelectedFilm
        {
            get => _selectedFilm;
            set => ChangeSelectedFilm(value);
        }

        /// <summary>
        /// Выбранный сезон
        /// </summary>
        public Season SelectedSeason
        {
            get => _selectedSeason;
            set => ChangeSelectedSeason(value);
        }

        /// <summary>
        /// Выбранный эпизод
        /// </summary>
        public Episode SelectedEpisode
        {
            get => _selectedEpisode;
            set => ChangeSelectedEpisode(value);
        }

        #endregion

        #region Lists data

        public bool IsFilmListEnable
        {
            get => _isFilmListEnable;
            set
            {
                _isFilmListEnable = value;
                NotifyOfPropertyChange(() => IsFilmListEnable);
            }
        }

        public bool IsSeasonListEnable
        {
            get => _isSeasonListEnable;
            set
            {
                _isSeasonListEnable = value;
                NotifyOfPropertyChange(() => IsSeasonListEnable);
            }
        }

        public bool IsEpisodeListEnable
        {
            get => _isEpisodeListEnable;
            set
            {
                _isEpisodeListEnable = value;
                NotifyOfPropertyChange(() => IsEpisodeListEnable);
            }
        }

        #endregion

        #region Visibility

        public Visibility SelectedFilmVisibility =>
            SelectedFilm == null
                ? Visibility.Hidden
                : Visibility.Visible;

        public Visibility SelectedSeasonVisibility =>
            SelectedSeason == null
                ? Visibility.Hidden
                : Visibility.Visible;

        public Visibility SelectedEpisodeVisibility =>
            SelectedEpisode == null
                ? Visibility.Hidden
                : Visibility.Visible;



        #endregion


        public bool HasChanges { get; }

        public void SaveChanges()
        {

        }

        public bool CanSaveChanges => true;

    }
}
