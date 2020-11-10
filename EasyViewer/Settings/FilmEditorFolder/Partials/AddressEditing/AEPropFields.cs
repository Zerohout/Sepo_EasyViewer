// ReSharper disable once CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using LibVLCSharp.Shared;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.GlobalMethods;
    using static Helpers.SystemVariables;

    public partial class AddressEditingViewModel : Screen
    {
        private AddressInfo _currentAddressInfo;
        private int? _newJumperNumber;
        private Jumper _selectedJumper;
        private Uri _newAddress;
        private bool _isInterfaceUnlocked = true;
        private BindableCollection<Jumper> _jumpers;
        private MediaPlayer _mediaPlayer;
        private LibVLC _libVlc;
        private TimeSpan _currentEpisodeTime;
        private BindableCollection<Jumper> _selectedJumpers = new BindableCollection<Jumper>();
        private Visibility _endValueVisibility;
        private Visibility _volumeValueVisibility;
        private const int RewindStep = 2500;


        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                _mediaPlayer = value;
                NotifyOfPropertyChange(() => MediaPlayer);
            }
        }

        public TimeSpan CurrentEpisodeTime
        {
            get => _currentEpisodeTime;
            set
            {
                _currentEpisodeTime = value;
                NotifyOfPropertyChange(() => CurrentEpisodeTime);
            }
        }

        private long _tempCurrentEpisodeTime;

        public long TempCurrentEpisodeTime
        {
            get => _tempCurrentEpisodeTime;
            set
            {
                _tempCurrentEpisodeTime = value;
                SetMediaTime(value);
                NotifyOfPropertyChange(() => TempCurrentEpisodeTime);
                
            }
        }

        private async void SetMediaTime(long time)
        {
            await Task.Run(() =>
            {
                MediaPlayer.Time = time;
                
            });
            NotifyOfPropertyChange(() => CanFastBackward);
            NotifyOfPropertyChange(() => CanFastForward);
            NotifyOfPropertyChange(() => CanPlay);
            NotifyOfPropertyChange(() => CanDecreaseVideoRate);
            NotifyOfPropertyChange(() => CanIncreaseVideoRate);
            NotifyOfPropertyChange(() => CanDefaultVideoRate);
            NotifyOfPropertyChange(() => CurrentEpisodeTime);
        }

        #region Свойства адреса

        /// <summary>
        /// Текущий адрес
        /// </summary>
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
        /// Строковый экземпляр адреса для отслеживания изменений
        /// </summary>
        public string AddressSnapshot { get; set; }

        /// <summary>
        /// Новый адрес
        /// </summary>
        public Uri NewAddress
        {
            get => _newAddress;
            set
            {
                _newAddress = value;
                NotifyOfPropertyChange(() => NewAddress);
            }
        }

        /// <summary>
        /// Флаг наличия изменений свойств адреса
        /// </summary>
        private bool HasAddressChanges =>
            IsEquals(CurrentAddressInfo, JsonConvert.DeserializeObject(AddressSnapshot)) is false ||
            CurrentAddressInfo.Link != NewAddress;

        #endregion

        #region Свойства джампера

        /// <summary>
        /// Список джамперов
        /// </summary>
        public BindableCollection<Jumper> Jumpers
        {
            get => _jumpers;
            set
            {
                _jumpers = new BindableCollection<Jumper>(value.OrderBy(j => j.Number).ToList());
                IsInterfaceUnlocked = true;
                NotifyOfPropertyChange(() => Jumpers);
                NotifyOfPropertyChange(() => CanRefreshJumpersConfig);
                NotifyOfPropertyChange(() => CanAddFirstJumper);
                NotifyOfPropertyChange(() => CanAddVolumeJumper);
            }
        }

        /// <summary>
        /// Выбранный джампер
        /// </summary>
        public Jumper SelectedJumper
        {
            get => _selectedJumper;
            set
            {
                _selectedJumper = value;
                if (value != null)
                {
                    JumperSnapshot = JsonConvert.SerializeObject(value);
                    NewJumperNumber = value.Number;
                    IsInterfaceUnlocked = false;
                }
                else
                {
                    IsInterfaceUnlocked = true;
                }

                NotifyOfPropertyChange(() => SelectedJumper);
                NotifyOfPropertyChange(() => CanSetJumperNumber);
            }
        }


        public BindableCollection<Jumper> SelectedJumpers
        {
            get => _selectedJumpers;
            set
            {
                _selectedJumpers = value;
                NotifyOfPropertyChange(() => SelectedJumpers);
                NotifyOfPropertyChange(() => CanRemoveJumper);
                NotifyOfPropertyChange(() => CanCancelSelection);
            }
        }

        /// <summary>
        /// СТроковый экземпляр джампера для отслеживания изменений
        /// </summary>
        public string JumperSnapshot { get; set; }

        /// <summary>
        /// Свойство Visibility интерфейса редактирования джампера
        /// </summary>
        public Visibility JumperEditingVisibility => IsInterfaceUnlocked != true
            ? Visibility.Visible
            : Visibility.Hidden;

        /// <summary>
        /// Новый номер джампера
        /// </summary>
        public int? NewJumperNumber
        {
            get => _newJumperNumber;
            set
            {
                if (value == null || value < 0)
                {
                    value = 1;
                }

                _newJumperNumber = value;
                NotifyOfPropertyChange(() => NewJumperNumber);
            }
        }

        /// <summary>
        /// Список типов фильма
        /// </summary>
        public BindableCollection<JumperMode> JumperModes =>
            new BindableCollection<JumperMode>(Enum.GetValues(typeof(JumperMode)).Cast<JumperMode>());

        /// <summary>
        /// Флаг наличия изменений свойств джампера
        /// </summary>
        private bool HasJumperChanges
        {
            get
            {
                if (SelectedJumper == null)
                {
                    return false;
                }

                return IsEquals(SelectedJumper, JsonConvert.DeserializeObject(JumperSnapshot)) is false
                       || SelectedJumper.Number != NewJumperNumber;
            }
        }

        public Visibility EndValueVisibility
        {
            get => _endValueVisibility;
            set
            {
                _endValueVisibility = value;
                NotifyOfPropertyChange(() => EndValueVisibility);
            }
        }

        public Visibility VolumeValueVisibility
        {
            get => _volumeValueVisibility;
            set
            {
                _volumeValueVisibility = value;
                NotifyOfPropertyChange(() => VolumeValueVisibility);
            }
        }

        #endregion

        public EpisodesEditorViewModel EEVM => Parent as EpisodesEditorViewModel;

        /// <summary>
        /// Флаг состояния разблокировки интерфейса (активация при изменении данных джампера)
        /// </summary>
        public bool IsInterfaceUnlocked
        {
            get => _isInterfaceUnlocked;
            set
            {
                _isInterfaceUnlocked = value;
                NotifyOfPropertyChange(() => IsInterfaceUnlocked);
                NotifyOfPropertyChange(() => JumperEditingVisibility);
            }
        }
    }
}