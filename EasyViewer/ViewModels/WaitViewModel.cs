namespace EasyViewer.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Helpers;
    using static Helpers.SystemVariables;

    public class WaitViewModel : Screen
    {
        private Visibility _addressesVisibility;
        private int _currentAddressNumber;
        private int _currentEpisodeValue;
        private LoadingStatus _currentLoadingStatus;
        private int _currentPercentValue;
        private int _currentSeasonValue;
        private TimeSpan _elapsedTime;
        private Visibility _episodesVisibility;
        private byte _g;
        private string _loadingStatus;
        private int _maxAddressNumber;
        private int _maxEpisodeValue;
        private int _maxPercentValue;
        private int _maxSeasonValue;
        private byte _r = 255;
        private TimeSpan _remainingTime;
        

        /// <summary>
        ///     Сброс цвета
        /// </summary>
        public void ResetRGB()
        {
            _r = 255;
            _g = 0;
        }

        /// <summary>
        ///     Сброс всех данных загрузки
        /// </summary>
        public void ResetLoadingData()
        {
            MaxSeasonValue = 0;
            CurrentSeasonValue = 0;
            MaxEpisodeValue = 0;
            CurrentEpisodeValue = 0;
            MaxAddressNumber = 0;
            CurrentAddressNumber = 0;
        }

        /// <summary>
        ///     Сбросить текущие данные
        /// </summary>
        /// <param name="isResetEpisodes">Если истина, сброс текущего значения эпизодов</param>
        /// <param name="isResetAddresses">Если истина, сброс текущего значения адресов</param>
        public void ResetCurrentLoadingData(bool isResetEpisodes = false, bool isResetAddresses = false)
        {
            if (isResetEpisodes is true) CurrentEpisodeValue = 0;
            if (isResetAddresses is true) CurrentAddressNumber = 0;
        }

        /// <summary>
        ///     Отмена операции
        /// </summary>
        public void CancelOperation()
        {
            AddingFilmCancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Получить цвет полосы прогресса
        /// </summary>
        /// <param name="currentValue">Текущее значение</param>
        /// <returns></returns>
        private SolidColorBrush GetRGBColor(int currentValue)
        {
            if (currentValue > 100) return new SolidColorBrush(Color.FromRgb(0, 255, 0));

            if (currentValue <= 50)
                _g = (byte) (currentValue * 5.1);
            else
                _r = (byte) (255 - (int) ((currentValue - 50) * 5.1));

            return new SolidColorBrush(Color.FromRgb(_r, _g, 0));
        }

        /// <summary>
        ///     Получение процентного соотношения числа a от числа b
        /// </summary>
        /// <param name="b">Число b</param>
        /// <param name="a">Число а</param>
        /// <returns></returns>
        public static int GetPercent(int b, int a)
        {
            if (b == 0) return 0;

            return (int) (a / (b / 100M));
        }

        #region Текстовые свойства отображения данных

        /// <summary>
        ///     Текущий статус загрузки данных
        /// </summary>
        public LoadingStatus CurrentLoadingStatus
        {
            get => _currentLoadingStatus;
            set
            {
                switch (value)
                {
                    case SystemVariables.LoadingStatus.CreatingSeasons:
                        LoadingStatus = "Создание сезонов 1/4";
                        EpisodesVisibility = Visibility.Hidden;
                        AddressesVisibility = Visibility.Hidden;
                        break;
                    case SystemVariables.LoadingStatus.CreatingEpisodes:
                        LoadingStatus = "Создание эпизодов 2/4";
                        EpisodesVisibility = Visibility.Visible;
                        AddressesVisibility = Visibility.Hidden;

                        break;
                    case SystemVariables.LoadingStatus.CreatingAddresses:
                        LoadingStatus = "Создание адресов 3/4";
                        EpisodesVisibility = Visibility.Visible;
                        AddressesVisibility = Visibility.Visible;
                        break;
                    case SystemVariables.LoadingStatus.AddingDurations:
                        LoadingStatus = "Добавление длительностей 4/4";
                        EpisodesVisibility = Visibility.Visible;
                        AddressesVisibility = Visibility.Visible;
                        break;
                }

                ResetLoadingData();
                CurrentPercentValue = 0;
                _currentLoadingStatus = value;
                ResetRGB();
                NotifyOfPropertyChange(() => CurrentLoadingStatus);
            }
        }

        public SolidColorBrush ProgressBarForeground => GetRGBColor(Procents);

        /// <summary>
        ///     Процентное соотношение прогресса операции
        /// </summary>
        public int Procents => GetPercent(_maxPercentValue, _currentPercentValue);

        /// <summary>
        ///     Примерное оставшееся время завершения операции
        /// </summary>
        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            set
            {
                if (_currentPercentValue <= 1)
                {
                    _remainingTime = TimeSpan.FromMilliseconds((long) (value.TotalMilliseconds * MaxPercentValue));
                }
                else
                {
                    var max = (long) (value.TotalMilliseconds / _currentPercentValue) * MaxPercentValue;
                    _remainingTime = TimeSpan.FromMilliseconds((long) (max - value.TotalMilliseconds));
                }

                NotifyOfPropertyChange(() => RemainingTime);
            }
        }

        /// <summary>
        ///     Прошешее время
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                NotifyOfPropertyChange(() => ElapsedTime);
            }
        }

        /// <summary>
        ///     Свойство Visibility отображения информации об эпизодах
        /// </summary>
        public Visibility EpisodesVisibility
        {
            get => _episodesVisibility;
            set
            {
                _episodesVisibility = value;
                NotifyOfPropertyChange(() => EpisodesVisibility);
            }
        }

        /// <summary>
        ///     Свойство Visibility отображения информации об адресах
        /// </summary>
        public Visibility AddressesVisibility
        {
            get => _addressesVisibility;
            set
            {
                _addressesVisibility = value;
                NotifyOfPropertyChange(() => AddressesVisibility);
            }
        }

        /// <summary>
        ///     Текущее значение для подсчета процентов
        /// </summary>
        public int CurrentPercentValue
        {
            get => _currentPercentValue;
            set
            {
                _currentPercentValue = value;
                NotifyOfPropertyChange(() => CurrentPercentValue);
            }
        }

        /// <summary>
        ///     Максимальное значение для подсчета процентов
        /// </summary>
        public int MaxPercentValue
        {
            get => _maxPercentValue;
            set
            {
                _maxPercentValue = value;
                NotifyOfPropertyChange(() => MaxPercentValue);
            }
        }

        /// <summary>
        ///     Номер текущего сезона
        /// </summary>
        public int CurrentSeasonValue
        {
            get => _currentSeasonValue;
            set
            {
                _currentSeasonValue = value;
                NotifyOfPropertyChange(() => CurrentSeasonValue);
            }
        }

        /// <summary>
        ///     Максимальное количество сезонов
        /// </summary>
        public int MaxSeasonValue
        {
            get => _maxSeasonValue;
            set
            {
                _maxSeasonValue = value;
                NotifyOfPropertyChange(() => MaxSeasonValue);
            }
        }

        /// <summary>
        ///     Номер текущего эпизода
        /// </summary>
        public int CurrentEpisodeValue
        {
            get => _currentEpisodeValue;
            set
            {
                _currentEpisodeValue = value;
                NotifyOfPropertyChange(() => CurrentEpisodeValue);
                NotifyOfPropertyChange(() => ProgressBarForeground);
                NotifyOfPropertyChange(() => RemainingTime);
                NotifyOfPropertyChange(() => Procents);
            }
        }

        /// <summary>
        ///     Максимальное количество эпизодов
        /// </summary>
        public int MaxEpisodeValue
        {
            get => _maxEpisodeValue;
            set
            {
                _maxEpisodeValue = value;
                NotifyOfPropertyChange(() => MaxEpisodeValue);
            }
        }

        /// <summary>
        ///     Номер текущего адреса
        /// </summary>
        public int CurrentAddressNumber
        {
            get => _currentAddressNumber;
            set
            {
                _currentAddressNumber = value;
                NotifyOfPropertyChange(() => CurrentAddressNumber);
            }
        }

        /// <summary>
        ///     Максимальное количество адресов
        /// </summary>
        public int MaxAddressNumber
        {
            get => _maxAddressNumber;
            set
            {
                _maxAddressNumber = value;
                NotifyOfPropertyChange(() => MaxAddressNumber);
            }
        }

        /// <summary>
        ///     Текущий статус загрузки
        /// </summary>
        public string LoadingStatus
        {
            get => _loadingStatus;
            set
            {
                _loadingStatus = value;
                NotifyOfPropertyChange(() => LoadingStatus);
            }
        }

        #endregion
    }
}