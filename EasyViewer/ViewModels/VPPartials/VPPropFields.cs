// ReSharper disable once CheckNamespace

namespace EasyViewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using LibVLCSharp.Shared;
    using LibVLCSharp.WPF;
    using MainMenu.ViewModels;
    using Models.FilmModels;
    using static Helpers.SystemVariables;

    public partial class VideoPlayerViewModel : Screen
    {
        private const int RewindStep = 2500;
        private readonly LibVLC _libVlc;
        private readonly MainMenuViewModel _mmvm;
        private Visibility _controlVisibility = Visibility.Collapsed;
        private Episode _currentEpisode;
        private TimeSpan _currentEpisodeTime;
        private MediaPlayer _mediaPlayer;
        private bool _topmost;
        private double _windowHeight = AppVal.WS.VPSize.Y;
        private WindowState _windowState;
        private Visibility _windowVisibility;
        private VideoView _videoView;

        private double _windowWidth = AppVal.WS.VPSize.X;
        private AddressInfo _currentAddressInfo;

        #region Window props

        /// <summary>
        ///     Главное представление
        /// </summary>
        public Window MainView { get; set; }

        /// <summary>
        ///     Свойство WindowState проигрывателя
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
        ///     Свойство Visibility проигрывателя
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
        ///     Свойство Visibility панели управления видеоплеера
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
        ///     Ширина проигрывателя
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
        ///     Высота проигрывателя
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
        ///     Значение определяющее будет ли проигрыватель поверх всех окон.
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

        /// <summary>
        ///     Медиа проигрыватель
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                _mediaPlayer = value;
                NotifyOfPropertyChange(() => MediaPlayer);
            }
        }

        /// <summary>
        ///     Текущее время эпизода
        /// </summary>
        public TimeSpan CurrentEpisodeTime
        {
            get => _currentEpisodeTime;
            set
            {
                _currentEpisodeTime = value;
                NotifyOfPropertyChange(() => CurrentEpisodeTime);
                NotifyOfPropertyChange(() => MediaPlayer);
            }
        }

        /// <summary>
        ///     Флаг показывающий состояние проигрывания фильма
        /// </summary>
        public bool IsPlaying => MediaPlayer?.IsPlaying ?? false;

        #endregion

        #region Episodes props

        /// <summary>
        ///     Количество просматриваемых эпизодов
        /// </summary>
        public int? WatchingEpisodesCount
        {
            get => _mmvm.WatchingEpisodesCount;
            private set => _mmvm.WatchingEpisodesCount = value;
        }

        /// <summary>
        ///     Список выбранных эпизодов
        /// </summary>
        public List<Episode> CheckedEpisodes { get; set; } = new List<Episode>();

        /// <summary>
        ///     Индекс текущего эпизода
        /// </summary>
        private int CurrentEpisodeIndex { get; set; } = -1;

        /// <summary>
        ///     Текущий (пригрываемый) эпизод
        /// </summary>
        public Episode CurrentEpisode
        {
            get => _currentEpisode;
            set
            {
                _currentEpisode = value;
                CurrentAddressInfo = value.AddressInfo;
                Jumpers = new List<Jumper>(CurrentAddressInfo.Jumpers);
                CurrentJumper = Jumpers.FirstOrDefault();
                NotifyOfPropertyChange(() => CurrentEpisode);
            }
        }

        public AddressInfo CurrentAddressInfo
        {
            get => _currentAddressInfo;
            set
            {
                _currentAddressInfo = value;
                NotifyOfPropertyChange(() => CurrentAddressInfo);
            }
        }

        /// <summary>
        ///     Флаг пропуска эпизода
        /// </summary>
        private bool IsEpisodeSkipped { get; set; }

        #endregion

        #region Jumpers props

        /// <summary>
        ///     Список джамперов текущего эпизода
        /// </summary>
        private List<Jumper> Jumpers { get; set; }

        /// <summary>
        ///     Текущий джампер
        /// </summary>
        private Jumper CurrentJumper { get; set; }

        #endregion
    }
}