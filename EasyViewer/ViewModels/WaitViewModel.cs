namespace EasyViewer.ViewModels
{
    using System;
    using System.Windows.Media;
    using Caliburn.Micro;

    public class WaitViewModel : Screen
    {
        private int _currentValue;

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

        private int _maximumValue;

        public int MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                NotifyOfPropertyChange(() => MaximumValue);
            }
        }


        public SolidColorBrush ProgressBarForeground => GetRGBColor(GetPercent(_maximumValue, _currentValue));
        
        public int Procents => GetPercent(_maximumValue, _currentValue);

        private TimeSpan _remainingTime;

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

        private byte r = 255;
        private byte g;

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
