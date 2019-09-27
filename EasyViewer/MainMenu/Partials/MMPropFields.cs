// ReSharper disable once CheckNamespace
namespace EasyViewer.MainMenu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using Models.FilmModels;
    using static Helpers.SystemVariables;

    public partial class MainMenuViewModel : Screen
    {
        private BindableCollection<Film> _films = new BindableCollection<Film>();
        private readonly Random rnd = new Random();
        private bool _isShutdownComp;
        private double _opacity = 1;
        private Uri _background = new Uri(MMBackgroundUri, UriKind.Relative);
        private Visibility _secretVisibility = Visibility.Collapsed;
        public VideoPlayerViewModel VideoPlayer { get; set; }
        private int _availableEpisodesCount;
        private int? _watchingEpisodesCount;
        private List<Episode> _checkedEpisodes = new List<Episode>();

        #region Window properties

        /// <summary>
        /// Прозрачность элементов и фона MainMenu
        /// </summary>
        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                NotifyOfPropertyChange(() => Opacity);
            }
        }

        /// <summary>
        /// Фон MainMenu
        /// </summary>
        public Uri Background
        {
            get => _background;
            set
            {
                _background = value;
                NotifyOfPropertyChange(() => Background);
            }
        }

        /// <summary>
        /// Свойство Visibility у секретных кнопок
        /// </summary>
        public Visibility SecretVisibility
        {
            get => _secretVisibility;
            set
            {
                _secretVisibility = value;
                NotifyOfPropertyChange(() => SecretVisibility);
            }
        }

        #endregion

        #region Episodes values

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
                NotifyOfPropertyChange(() => CanAddEpisodes);
                NotifyOfPropertyChange(() => CanRemoveEpisodes);
            }
        }


        /// <summary>
        /// Список выбранных эпизодов
        /// </summary>
        public List<Episode> CheckedEpisodes
        {
            get => _checkedEpisodes;
            set
            {
                _checkedEpisodes = value;
                SetEpisodesValues();
                NotifyOfPropertyChange(() => CanStart);
                NotifyOfPropertyChange(() => CheckedEpisodes);
            }
        }

        /// <summary>
        /// Количество доступных к просмотру эпизодов
        /// </summary>
        public int AvailableEpisodesCount
        {
            get => _availableEpisodesCount;
            set
            {
                _availableEpisodesCount = value;
                NotifyOfPropertyChange(() => AvailableEpisodesCount);
            }
        }


        /// <summary>
        /// Количество эпизодов к просмотру
        /// </summary>
        public int? WatchingEpisodesCount
        {
            get => _watchingEpisodesCount;
            set
            {
                _watchingEpisodesCount = value == null
                    ? 0
                    : value > CheckedEpisodes.Count
                        ? _availableEpisodesCount
                        : value;

                if (VideoPlayer != null)
                {
                    NotifyOfPropertyChange(() => VideoPlayer.WatchingEpisodesCount);
                }

                NotifyOfPropertyChange(() => WatchingEpisodesCount);
                NotifyOfPropertyChange(() => CanStart);
                NotifyOfPropertyChange(() => EndTime);
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        /// <summary>
        /// Текст TextBlock'а Осталось серий/Серий к просмотру
        /// </summary>
        public string EpisodesCountRemainingString => VlcPlayer == null
            ? "Эпизодов к просмотру:"
            : "Эпизодов осталось:";

        #endregion


        public bool IsViewingEnded { get; set; } = false;
        
        #region Свойства связанные со временем

        /// <summary>
        /// Конечное время просмотра указанного числа серий
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                var duration = TimeSpan.FromSeconds(CheckedEpisodes
                                                    .Take(WatchingEpisodesCount ?? 0)
                                                    .Sum(ce => ce.ActualDuration.TotalSeconds));

                return DateTime.Now.TimeOfDay + duration;

            }
        }

        /// <summary>
        /// Конечная дата просмотра указанного числа серий
        /// </summary>
        public DateTime EndDate => DateTime.Now.AddDays(EndTime.Days);

        #endregion

        /// <summary>
        /// Флаг выключения компьютера
        /// </summary>
        public bool IsShutdownComp
        {
            get => _isShutdownComp;
            set
            {
                _isShutdownComp = value;
                NotifyOfPropertyChange(() => IsShutdownComp);
            }
        }

    }
}
