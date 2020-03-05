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
        private bool _isNightHelperShutdownRemarkExpand;


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

                Films.Where(s => s.Checked)
                       .ToList().ForEach(s => result.AddRange(s.GetEpisodes(
                                                                  true, WatchingSettings.NonRepeatDaysInterval ?? -1)));
                return result;
            }
        }

        /// <summary>
        /// Флаг состояния описания интеллектуального выключения
        /// </summary>
        public bool IsNightHelperShutdownRemarkExpand
        {
            get => _isNightHelperShutdownRemarkExpand;
            set
            {
                _isNightHelperShutdownRemarkExpand = value;
                NotifyOfPropertyChange(() => IsNightHelperShutdownRemarkExpand);
                NotifyOfPropertyChange(() => NightHelperShutdownRemarkVisibility);
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
        /// Свойство Visibility описания интеллектуального выключения
        /// </summary>
        public Visibility NightHelperShutdownRemarkVisibility =>
            _isNightHelperShutdownRemarkExpand
                ? Visibility.Visible
                : Visibility.Collapsed;

        /// <summary>
        /// Фактическая длительность эпизодов по умолчанию
        /// </summary>
        public TimeSpan DefaultEpisodesActualDuration =>
            TimeSpan.FromSeconds(CheckedEpisodes.Take(WatchingSettings.DefaultEpCount ?? 1)
                                                .Sum(ce => ce.Address.ActualDuration.TotalSeconds));

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
