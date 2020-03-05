namespace EasyViewer.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Media;
    using Caliburn.Micro;
	using static Helpers.SystemVariables;
    using Timer = System.Timers.Timer;

    public class WaitViewModel : Screen
    {
        private int _currentValue;
        private int _maximumValue;
		private int _currentAddressNumber;
		private int _maxAddressNumber;
        private TimeSpan _remainingTime;
		private TimeSpan _elapsedTime;
        private byte r = 255;
        private byte g;

		/// <summary>
		/// Номер текущего эпизода
		/// </summary>
		public int CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                NotifyOfPropertyChange(() => CurrentValue);
                NotifyOfPropertyChange(() => ProgressBarForeground);
                NotifyOfPropertyChange(() => RemainingTime);
                NotifyOfPropertyChange(() => Procents);
            }
        }

		/// <summary>
		/// Максимальное количество эпизодов
		/// </summary>
        public int MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                NotifyOfPropertyChange(() => MaximumValue);
            }
        }

		/// <summary>
		/// Номер текущего адреса
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
		/// Максимальное количество адресов
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


		public SolidColorBrush ProgressBarForeground => GetRGBColor(GetPercent(_maximumValue, _currentValue));
        
		/// <summary>
		/// Процентное соотношение прогресса операции
		/// </summary>
        public int Procents => GetPercent(_maximumValue, _currentValue);

		/// <summary>
		/// Примерное оставшееся время завершения операции
		/// </summary>
        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            set
            {
                if (_currentValue < 1)
                {
                    _remainingTime = TimeSpan.FromMilliseconds((long)(value.TotalMilliseconds * _maximumValue));
                }
                else
                {
                    var max = (long)(value.TotalMilliseconds / _currentValue) * _maximumValue;
                    _remainingTime = TimeSpan.FromMilliseconds((long)(max - value.TotalMilliseconds));
                }

                NotifyOfPropertyChange(() => RemainingTime);
            }
        }

		/// <summary>
		/// Прошешее время
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

		public Timer Timer { get; set; }
		public Stopwatch Stopwatch { get; set; }

		public void CancelOperation()
		{
			AddingFilmCancellationTokenSource.Cancel();
		}

		/// <summary>
		/// Получить цвет полосы прогресса
		/// </summary>
		/// <param name="currentValue">Текущее значение</param>
		/// <returns></returns>
		private SolidColorBrush GetRGBColor(int currentValue)
        {
            if (currentValue > 100) return new SolidColorBrush(Color.FromRgb(0, 255, 0));


            if (currentValue <= 50)
            {
                g = (byte)(currentValue * 5.1);
            }
            else
            {
                r = (byte)(255 - (int)((currentValue - 50) * 5.1));
            }

            return new SolidColorBrush(Color.FromRgb(r, g, 0));
        }

        /// <summary>
        /// Получение процентного соотношения числа a от числа b
        /// </summary>
        /// <param name="b">Число b</param>
        /// <param name="a">Число а</param>
        /// <returns></returns>
        public static int GetPercent(int b, int a)
        {
            if (b == 0) return 0;

            return (int)(a / (b / 100M));
        }
    }
}
