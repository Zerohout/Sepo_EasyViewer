// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.GlobalMethods;

    public partial class SeasonsEditingViewModel : Screen
    {
        private Season _currentSeason;
        private Episode _selectedEpisode;
        private string _seasonSnapshot;
        private int? _defaultAddingEpisodeValue = 1;
        private int? _jumperStartTime;
        private int? _jumperEndTime;

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
        public string SeasonSnapshot
        {
            get => _seasonSnapshot;
            set
            {
                _seasonSnapshot = value;
                NotifyOfPropertyChange(() => SeasonSnapshot);
            }
        }

        /// <summary>
        /// Изображение сезона
        /// </summary>
        public BitmapImage Logo => LoadImage(CurrentSeason.ImageBytes.ToArray());

        #endregion

        #region Свойства эпизодов

        /// <summary>
        /// Список эпизодов текущего сезона
        /// </summary>
        public BindableCollection<Episode> Episodes => new BindableCollection<Episode>(CurrentSeason.Episodes);

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
                NotifyOfPropertyChange(() => CanEditEpisode);

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
            IsEquals(CurrentSeason, JsonConvert.DeserializeObject<Film>(SeasonSnapshot)) is false;

        #endregion

        #region Свойства создания эпизодов по умолчанию

        /// <summary>
        /// Флаг активации/деактивации настроек создания эпизодов по умолчанию
        /// </summary>
        public bool IsDefSettingsEnabled { get; set; }

        /// <summary>
        /// Свойство Visibility настроек создания эпизодов по умолчанию
        /// </summary>
        public Visibility DefSettingsVisibility => IsDefSettingsEnabled
            ? Visibility.Visible
            : Visibility.Hidden;

        /// <summary>
        /// Количество создаваемых эпизодов по умолчанию за раз
        /// </summary>
        public int? DefaultAddingEpisodeValue
        {
            get => _defaultAddingEpisodeValue;
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

                _defaultAddingEpisodeValue = value;
                NotifyOfPropertyChange(() => DefaultAddingEpisodeValue);
            }
        }

        #endregion

        #region Свойства джампера

        /// <summary>
        /// Флаг активации/деактивации создания первого джампера
        /// </summary>
        public bool IsFirstJumperEnabled { get; set; }

        /// <summary>
        /// Свойство Visibility настроек создания первого джампера
        /// </summary>
        public Visibility FirstJumperSettingsVisibility => IsFirstJumperEnabled
            ? Visibility.Visible
            : Visibility.Collapsed;

        /// <summary>
        /// Время начала джампера
        /// </summary>
        public int? JumperStartTime
        {
            get => _jumperStartTime;
            set
            {
                if (value == null)
                {
                    value = 0;
                }
                _jumperStartTime = value;
                NotifyOfPropertyChange(() => JumperStartTime);
            }
        }


        /// <summary>
        /// Время окончания джампера
        /// </summary>
        public int? JumperEndTime
        {
            get => _jumperEndTime;
            set
            {
                if (value == null)
                {
                    value = 1;
                }

                if (value < _jumperStartTime)
                {
                    value = _jumperStartTime + 1;
                }
                _jumperEndTime = value;
                NotifyOfPropertyChange(() => JumperEndTime);
            }
        }

        #endregion

    }
}