// ReSharper disable once CheckNamespace
namespace EasyViewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using MainMenu.ViewModels;
    using Models.FilmModels;
    using Models.SettingModels;
    using Vlc.DotNet.Core;
    using Vlc.DotNet.Wpf;
    using static Helpers.SystemVariables;

    public partial class VideoPlayerViewModel : Screen
    {
        private Random rnd;
        private WindowState _windowState;
        private Visibility _windowVisibility;
        private Visibility _controlVisibility = Visibility.Collapsed;
        private TimeSpan _currentEpisodeTime;
        private Episode _currentEpisode;

        private double _windowWidth = AppVal.WS.VPSize.X;
        private double _windowHeight = AppVal.WS.VPSize.Y;
        private bool _topmost;
        private int _currentJumperIndex = -1;

        #region Window props

        /// <summary>
        /// Главное представление
        /// </summary>
        public Window MainView { get; set; }

        /// <summary>
        /// Свойство WindowState проигрывателя
        /// </summary>
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);
            }
        }

        /// <summary>
        /// Свойство Visibility проигрывателя
        /// </summary>
        /// <remarks>Используется для разворачивания на полный экран</remarks> 

        public Visibility WindowVisibility
        {
            get => _windowVisibility;
            set
            {
                _windowVisibility = value;
                NotifyOfPropertyChange(() => WindowVisibility);
            }
        }

        /// <summary>
        /// Свойство Visibility панели управления видеоплеера
        /// </summary>
        public Visibility ControlVisibility
        {
            get => _controlVisibility;
            set
            {
                _controlVisibility = value;
                NotifyOfPropertyChange(() => ControlVisibility);
            }
        }

        /// <summary>
        /// Ширина проигрывателя
        /// </summary>
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                NotifyOfPropertyChange(() => WindowWidth);
            }
        }

        /// <summary>
        /// Высота проигрывателя
        /// </summary>
        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                NotifyOfPropertyChange(() => WindowHeight);
            }
        }

        /// <summary>
        /// Значение определяющее будет ли проигрыватель поверх всех окон.
        /// </summary>
        public bool Topmost
        {
            get => _topmost;
            set
            {
                _topmost = value;
                NotifyOfPropertyChange(() => Topmost);
            }
        }

        #endregion

        #region VideoPlayer props

        ///// <summary>
        ///// Экземпляр VlcControl
        ///// </summary>
        //public VlcControl Vlc
        //{
        //    get => _vlc;
        //    set
        //    {
        //        _vlc = value;
        //        NotifyOfPropertyChange(() => Vlc);
        //    }
        //}

        ///// <summary>
        ///// Видеоплеер
        ///// </summary>
        //public VlcMediaPlayer VideoPlayer => _vlc.SourceProvider.MediaPlayer;

        /// <summary>
        /// Настройки просмотра
        /// </summary>
        public WatchingSettings WS => AppVal.WS;

        /// <summary>
        /// Текущее время эпизода
        /// </summary>
        public TimeSpan CurrentEpisodeTime
        {
            get => _currentEpisodeTime;
            set
            {
                _currentEpisodeTime = value;
                NotifyOfPropertyChange(() => CurrentEpisodeTime);
                NotifyOfPropertyChange(() => VlcPlayer);
            }
        }

        /// <summary>
        /// Модель представления главного меню
        /// </summary>
        private readonly MainMenuViewModel MMVM;

        /// <summary>
        /// Флаг показывающий состояние проигрывания фильма
        /// </summary>
        public bool IsPlaying => VlcPlayer?.IsPlaying() ?? false;

        #endregion

        #region Episodes props

        /// <summary>
        /// Количество просматриваемых эпизодов
        /// </summary>
        public int? WatchingEpisodesCount
        {
            get => MMVM.WatchingEpisodesCount;
            private set => MMVM.WatchingEpisodesCount = value;
        }

        /// <summary>
        /// Список выбранных эпизодов
        /// </summary>
        public List<Episode> CheckedEpisodes { get; set; } = new List<Episode>();

        /// <summary>
        /// Индекс текущего эпизода
        /// </summary>
        private int CurrentEpisodeIndex { get; set; }

        /// <summary>
        /// Текущий (пригрываемый) эпизод
        /// </summary>
        public Episode CurrentEpisode
        {
            get => _currentEpisode;
            set
            {
                _currentEpisode = value;
                CurrentJumperIndex = Jumpers.Count > 0
                    ? 0
                    : -1;
                NotifyOfPropertyChange(() => CurrentEpisode);
            }
        }

        /// <summary>
        /// Флаг пропуска эпизода
        /// </summary>
        private bool IsEpisodeSkipped { get; set; }

        #endregion

        #region Jumpers props

        /// <summary>
        /// Список джамперов текущего эпизода
        /// </summary>
        private List<Jumper> Jumpers => CurrentEpisode?.Jumpers ?? new List<Jumper>();

        /// <summary>
        /// Индекс текущего джампера
        /// </summary>
        public int CurrentJumperIndex
        {
            get => _currentJumperIndex;
            set
            {
                CurrentJumper = value < 0
                    ? null
                    : Jumpers[value];

                _currentJumperIndex = value;
                NotifyOfPropertyChange(() => CurrentJumperIndex);
            }
        }

        /// <summary>
        /// Текущий джампер
        /// </summary>
        private Jumper CurrentJumper { get; set; }

        #endregion

    }
}
