namespace EasyViewer.Models.SettingModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using LiteDB;
    using Newtonsoft.Json;

    /// <summary>
    /// Настройки просмотра
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class WatchingSettings : PropertyChangedBase
    {
        private int? _defaultEpCount = 0;
        private int? _nonRepeatDaysInterval = 30;
        private int? _lastWatchedEpInRow = 101;
        private bool _randomWatching = true;
        private int? _randomMixCount = 1;
        private bool _watchingInRow;
        private int? _nightHelperStartTime = 0;
        private int? _nightHelperEndTime = 4;
        private bool _nightHelperShutdown;
        private bool _isEpisodeChainActive;
        
        /// <summary>
        /// Идентификационный номер настроек просмотра
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        #region Настройки видеоплеера

        /// <summary>
        /// Начальная позиция видеоплеера
        /// </summary>
        public Point VPStartupPos { get; set; } = new Point(200, 100);
        /// <summary>
        /// Размер видеоплеера
        /// </summary>
        public Point VPSize { get; set; } = new Point(400, 300);

        #endregion

        /// <summary>
        /// Количество эпизодов к просмотру по умолчанию при запуске программы
        /// </summary>
        public int? DefaultEpCount
        {
            get => _defaultEpCount;
            set
            {
                if (_defaultEpCount == value) return;
                if (value == null || value < 0) value = 0;

                _defaultEpCount = value;
                NotifyOfPropertyChange(() => DefaultEpCount);
            }
        }

        /// <summary>
        /// Интервал между просмотренными эпизодами
        /// </summary>
        public int? NonRepeatDaysInterval
        {
            get => _nonRepeatDaysInterval;
            set
            {
                var maxDays = DateTime.IsLeapYear(DateTime.Now.Year)
                    ? 366
                    : 365;

                if (_nonRepeatDaysInterval == value) return;

                if (value == null ||
                    value < -1)
                {
                    value = -1;
                }
                else if (value > maxDays)
                {

                    value = maxDays;
                }

                _nonRepeatDaysInterval = value;
                NotifyOfPropertyChange(() => NonRepeatDaysInterval);
            }
        }

        #region Values

        /// <summary>
        /// Полный номер последнего просмотренного эпизода в режиме Подряд
        /// </summary>
        public int? LastWatchedEpInRow
        {
            get => _lastWatchedEpInRow;
            set
            {
                if (value == null || value < 101)
                {
                    value = 101;
                }

                _lastWatchedEpInRow = value;
                NotifyOfPropertyChange(() => LastWatchedEpInRow);
            }
        }

        /// <summary>
        /// Флаг статуса активации цепи эпизодов
        /// </summary>
        public bool IsEpisodeChainActive
        {
            get => _isEpisodeChainActive;
            set
            {
                _isEpisodeChainActive = value;
                NotifyOfPropertyChange(() => IsEpisodeChainActive);
            }
        }

        /// <summary>
        /// Количество смешиваний выбранных эпизодов (экспериментальная функция)
        /// </summary>
        public int? RandomMixCount
        {
            get => _randomMixCount;
            set
            {
                if (_randomMixCount == value)
                    return;

                _randomMixCount = (value == null || value <= 0)
                    ? 1
                    : value > 10
                        ? 10
                        : value;

                NotifyOfPropertyChange(() => RandomMixCount);
            }
        }
        
        /// <summary>
        /// Флаг интеллектуального выключения
        /// </summary>
        public bool NightHelperShutdown
        {
            get => _nightHelperShutdown;
            set
            {
                _nightHelperShutdown = value;
                NotifyOfPropertyChange(() => NightHelperShutdown);
            }
        }
        
        /// <summary>
        /// Начальное время интеллектуального выключения
        /// </summary>
        public int? NightHelperStartTime
        {
            get => _nightHelperStartTime;
            set
            {
                if (value == _nightHelperEndTime)
                {
                    value--;
                    NightHelperEndTime++;
                }

                if (value == null || value < 0) value = 0;
                if (value > 23) value = 23;

                _nightHelperStartTime = value;
                NotifyOfPropertyChange(() => NightHelperStartTime);
            }
        }

        /// <summary>
        /// Конечное время интеллектуального выключения
        /// </summary>
        public int? NightHelperEndTime
        {
            get => _nightHelperEndTime;
            set
            {
                if (value == _nightHelperStartTime)
                {
                    value++;
                    NightHelperStartTime--;
                }

                if (value == null || value < 0) value = 0;
                if (value > 23) value = 23;

                _nightHelperEndTime = value;
                NotifyOfPropertyChange(() => NightHelperEndTime);
            }
        }

        /// <summary>
        /// Флаг просмотра эпизодов подряд
        /// </summary>
        public bool WatchingInRow
        {
            get => _watchingInRow;
            set
            {
                _watchingInRow = value;
                NotifyOfPropertyChange(() => WatchingInRow);
            }
        }

        /// <summary>
        /// Флаг просмотра эпизодов в случайном порядке
        /// </summary>
        public bool RandomWatching
        {
            get => _randomWatching;
            set
            {
                _randomWatching = value;
                NotifyOfPropertyChange(() => RandomWatching);
            }
        }

       
        

        #endregion
        
    }
}
