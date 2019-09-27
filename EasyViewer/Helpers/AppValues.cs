namespace EasyViewer.Helpers
{
    using System;
    using Caliburn.Micro;
    using Models.SettingModels;

    public class AppValues : PropertyChangedBase
    {
        private static int? _episodesCount = 4;
        private static int _availableEpisodesCount;

        /// <summary>
        /// Настройки просмотра
        /// </summary>
        public WatchingSettings WS = new WatchingSettings();
        public readonly DateTime ResetTime = new DateTime(2019, 01, 01);


        /// <summary>
        /// Количество доступных эпизодов к просмотру
        /// </summary>
        public int AvailableEpisodesCount
        {
            get => _availableEpisodesCount;
            set
            {
                _availableEpisodesCount = value;
                if (_episodesCount > value)
                {
                    EpisodesCount = value;
                    NotifyOfPropertyChange(() => _availableEpisodesCount);

                }
            }
        }

        /// <summary>
        /// Текущее количество эпизодов к просмотру
        /// </summary>
        public int? EpisodesCount
        {
            get => _episodesCount > _availableEpisodesCount
                ? _availableEpisodesCount
                : _episodesCount;
            set
            {
                if (_episodesCount == value) return;

                if (value == null || value < 0)
                {
                    value = 0;
                }

                if (value > AvailableEpisodesCount)
                {
                    value = AvailableEpisodesCount;
                }
                _episodesCount = value;

                NotifyOfPropertyChange(() => _episodesCount);
            }
        }
    }
}
