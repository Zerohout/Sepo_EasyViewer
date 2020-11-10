// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.WatchingSettingsFolder.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;
    using Models.SettingModels;

    public partial class WatchingSettingsViewModel : Screen
    {
        private WatchingSettings _watchingSettings;
        private WatchingSettings _tempWS;
        private BindableCollection<Film> _films = new BindableCollection<Film>();
        private Film _selectedGlobalResetFilm;
        private string _selectedNoneRepeatTimeType;
        private int? _noneRepeatTimeCount;


        public ICollection<string> NoneRepeatTimeTypeList => new List<string>
        {
            "никогда",
            "дней",
            "всегда"
        };

        /// <summary>
        /// Выбранный тип времени для запрета повторов просмотренных эпизодов
        /// </summary>
        public string SelectedNoneRepeatTimeType
        {
            get => _selectedNoneRepeatTimeType;
            set
            {
                if(_selectedNoneRepeatTimeType == value)
                    return;

                var tempValue = _selectedNoneRepeatTimeType;
                _selectedNoneRepeatTimeType = value;
                NotifyOfPropertyChange(() => SelectedNoneRepeatTimeType);

                var temp = ConvertNonRepeatTime(tempValue);

                if(temp == null)
                    return;

                NoneRepeatTimeCount = temp;
            }
        }

        /// <summary>
        /// Значение времени, в течение которого просмотренный эпизод не будет повторяться
        /// </summary>
        public int? NoneRepeatTimeCount
        {
            get => _noneRepeatTimeCount;
            set
            {
                if(_noneRepeatTimeCount == value)
                    return;

                _noneRepeatTimeCount = SetNonRepeatTime(value);
                NotifyOfPropertyChange(() => NoneRepeatTimeCount);
            }
        }

        

        /// <summary>
        /// Общие настройки
        /// </summary>
        public WatchingSettings WatchingSettings
        {
            get => _watchingSettings;
            set
            {
                _watchingSettings = value;
                NotifyOfPropertyChange(() => WatchingSettings);
                NotifyOfPropertyChange(() => HasChanges);
            }
        }

        /// <summary>
        /// Временное свойство для отслеживания изменений
        /// </summary>
        public WatchingSettings TempWS
        {
            get => _tempWS;
            set
            {
                _tempWS = value;
                NotifyOfPropertyChange(() => TempWS);
            }
        }

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
        /// Список доступных эпизодов к просмотру
        /// </summary>
        public List<Episode> CheckedEpisodes
        {
            get
            {
                var result = new List<Episode>();

                Films.Where(f => f.Checked)
                    .ToList().ForEach(f => result.AddRange(f.Episodes.Where(e => e.Season.Checked && e.Checked && DateTime.Now.Subtract(
                        e.LastDateViewed).TotalDays > (WatchingSettings.NonRepeatDaysInterval ?? -1))));
                return result;
            }
        }

        /// <summary>
        /// Выбранный фильм для сброса даты последнего просмотра его эпизодов
        /// </summary>
        public Film SelectedGlobalResetFilm
        {
            get => _selectedGlobalResetFilm;
            set
            {
                _selectedGlobalResetFilm = value;
                NotifyOfPropertyChange(() => SelectedGlobalResetFilm);
                NotifyOfPropertyChange(() => CanResetLastDateViewed);
            }
        }

        /// <summary>
        /// Фактическая длительность эпизодов по умолчанию
        /// </summary>
        public TimeSpan DefaultEpisodesActualDuration =>
            TimeSpan.FromSeconds(CheckedEpisodes.Take(WatchingSettings.DefaultEpCount ?? 1)
                                                .Sum(episode =>
                                                {
                                                    var address = episode.AddressInfo;
                                                    var actDur = address.ActualDuration;
                                                    address.AddressActualDuration = new TimeSpan();
                                                    return actDur.TotalSeconds;
                                                }));

        /// <summary>
        /// Видимость элементов зависимых от просмотра эпизодов в случайном порядке
        /// </summary>
        public Visibility RandomEnabledVisibility => WatchingSettings.RandomWatching
            ? Visibility.Visible
            : Visibility.Hidden;

        /// <summary>
        /// Видимость элементов зависимых от просмотра эпизодов по порядку
        /// </summary>
        public Visibility WatchingInRowVisibility => WatchingSettings.WatchingInRow
            ? Visibility.Visible
            : Visibility.Hidden;



        /// <summary>
        /// Видимость элементов зависимых от статуса интеллектуального выключения
        /// </summary>
        public Visibility NightHelperSettingsVisibility =>
            WatchingSettings.NightHelperShutdown
                ? Visibility.Visible
                : Visibility.Collapsed;

        /// <summary>
        /// Флаг наличия изменений
        /// </summary>
        public bool HasChanges => GlobalMethods.IsEquals(WatchingSettings, TempWS) is false;
    }
}
