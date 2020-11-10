// ReSharper disable CheckNamespace

namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;
    using FilmEditorFolder.ViewModels;
    using Models.FilmModels;


    public partial class ViewingsSettingsViewModel : Screen
    {
        private BindableCollection<Film> _films = new BindableCollection<Film>();
        private BindableCollection<Season> _seasons = new BindableCollection<Season>();
        private BindableCollection<Episode> _episodes = new BindableCollection<Episode>();
        private BindableCollection<AddressInfo> _addressInfoList = new BindableCollection<AddressInfo>();

        private Film _selectedFilm;
        private Season _selectedSeason;
        private Episode _selectedEpisode;
        private AddressInfo _selectedAddressInfo;

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
                NotifyOfPropertyChange(() => Episodes);
            }
        }

        public BindableCollection<AddressInfo> AddressInfoList
        {
            get => _addressInfoList;
            set
            {
                _addressInfoList = value;
                NotifyOfPropertyChange(()=> AddressInfoList);
            }
        }

        /// <summary>
        /// Выбранный фильм
        /// </summary>
        public Film SelectedFilm
        {
            get => _selectedFilm;
            set
            {
                _selectedFilm = value;
                if(AddressInfoList.Count > 0) AddressInfoList = new BindableCollection<AddressInfo>();
                if (Episodes.Count > 0) Episodes = new BindableCollection<Episode>();
                Seasons = value != null
                    ? new BindableCollection<Season>(value.Seasons)
                    : new BindableCollection<Season>();
                NotifyFilmData();
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
                if(AddressInfoList.Count > 0) AddressInfoList = new BindableCollection<AddressInfo>();
                Episodes = value == null
                    ? new BindableCollection<Episode>()
                    : new BindableCollection<Episode>(value.Episodes);
                NotifySeasonData();
            }
        }

        /// <summary>
        /// Выбранный эпизод
        /// </summary>
        public Episode SelectedEpisode
        {
            get => _selectedEpisode;
            set
            {
                _selectedEpisode = value;
                AddressInfoList = value == null
                    ? new BindableCollection<AddressInfo>()
                    : new BindableCollection<AddressInfo>(value.AddressInfoList); 
                NotifyEpisodeData();
            }
        }

        public AddressInfo SelectedAddressInfo
        {
            get => _selectedAddressInfo;
            set
            {
                _selectedAddressInfo = value;
                NotifyOfPropertyChange(()=> SelectedAddressInfo);
                NotifyOfPropertyChange(() => CanCancelVoiceOverSelection);
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
    }
}