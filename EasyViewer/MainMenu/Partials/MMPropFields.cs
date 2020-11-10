// ReSharper disable once CheckNamespace

namespace EasyViewer.MainMenu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using LibVLCSharp.Shared;
    using Models.FilmModels;
    using static Helpers.SystemVariables;

    public partial class MainMenuViewModel : Screen
    {
        private BindableCollection<Film> _films = new BindableCollection<Film>();
        private bool _isShutdownComp;
        private double _opacity = 1;
        private Uri _background = new Uri(MMBackgroundUri, UriKind.Relative);
        private Visibility _secretVisibility = Visibility.Collapsed;

        private int _availableEpisodesCount;
        private int? _watchingEpisodesCount;
        private List<Episode> _checkedEpisodes = new List<Episode>();
        private Film _selectedRemovingFilm;

        private BindableCollection<string> _commandList = new BindableCollection<string>
        {
            "Добавить \"Южный Парк\"",
            "Проверить длительности эпизодов",
            "Проверить содержимое фильма \"Южный Парк\"",
            "Удалить все фильмы"
        };

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

        public VideoPlayerViewModel VideoPlayer { get; set; }

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
                NotifyOfPropertyChange(() => CanAddAddSouthPark);
                NotifyOfPropertyChange(() => CanRemoveFilms);
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        /// <summary>
        /// Выбранный фильм для удаления
        /// </summary>
        public Film SelectedRemovingFilm
        {
            get => _selectedRemovingFilm;
            set
            {
                _selectedRemovingFilm = value;
                NotifyOfPropertyChange(() => SelectedRemovingFilm);
                NotifyOfPropertyChange(() => CanRemoveSelectedFilm);
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
        //TODO Убрать рекусрию (цикличность оповещений свойств)
        public int? WatchingEpisodesCount
        {
            get => _watchingEpisodesCount;
            set
            {
                if (_watchingEpisodesCount == value) return;
                _watchingEpisodesCount = value == null
                    ? 0
                    : value > _checkedEpisodes.Count
                        ? _availableEpisodesCount
                        : value;
                _endTime = new TimeSpan();

                if (VideoPlayer != null)
                {
                    NotifyOfPropertyChange(() => VideoPlayer.WatchingEpisodesCount);
                }

                NotifyOfPropertyChange(() => CanStart);
                NotifyOfPropertyChange(() => EndTime);
                NotifyOfPropertyChange(() => EndDate);
                NotifyOfPropertyChange(() => WatchingEpisodesCount);
            }
        }

        /// <summary>
        /// Текст TextBlock'а Осталось серий/Серий к просмотру
        /// </summary>
        public string EpisodesCountRemainingString => VideoPlayer == null
            ? "Эпизодов к просмотру:"
            : "Эпизодов осталось:";

        #endregion

        #region Свойства связанные со временем

        private int _endTimeDays;
        private TimeSpan _endTime;

        /// <summary>
        /// Конечное время просмотра указанного числа серий
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                if (_endTime > new TimeSpan()) return _endTime;
                
                var duration = TimeSpan.FromSeconds(_checkedEpisodes
                    .Take(WatchingEpisodesCount ?? 0)
                    .Sum(episode =>
                    {
                        var address = episode.AddressInfo;
                        var actDur = address.ActualDuration;
                        address.AddressActualDuration = new TimeSpan();
                        return actDur.TotalSeconds;
                    }));

                _endTime = (DateTime.Now.TimeOfDay + duration);
                _endTimeDays = _endTime.Days;
                return _endTime;
            }
        }

        /// <summary>
        /// Конечная дата просмотра указанного числа серий
        /// </summary>
        public DateTime EndDate => DateTime.Now.AddDays(_endTimeDays);

        #endregion

        /// <summary>
        /// Список команд панели разработчика
        /// </summary>
        public BindableCollection<string> CommandList
        {
            get => _commandList;
            set
            {
                _commandList = value;
                NotifyOfPropertyChange(() => CommandList);
            }
        }

        /// <summary>
        /// Выбранная команда
        /// </summary>
        public string SelectedCommand { get; set; }

        /// <summary>
        /// Флаг завершения просмотра серий
        /// </summary>
        public bool IsViewingEnded { get; set; }

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