
namespace EasyViewer.Models.FilmModels
{
    using System;
    using static Helpers.SystemVariables;
    /// <summary>
    /// Джампер (выполнение операции на определенном отрезке времени)
    /// </summary>
    [Serializable]
    public class Jumper
    {
        /// <summary>
        /// Номер джампера в последовательности
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Время начала джампера
        /// </summary>
        public TimeSpan StartTime { get; set; }
        /// <summary>
        /// Время окончания джампера
        /// </summary>
        public TimeSpan EndTime { get; set; }
        /// <summary>
        /// Длительность джаспера
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;
        /// <summary>
        /// Режим джампера
        /// </summary>
        public JumperMode JumperMode { get; set; }
        /// <summary>
        /// Флаг статуса работы джампера
        /// </summary>
        public bool IsWorking { get; set; }

        #region Управление звуком

        /// <summary>
        /// Значение повышения громкости
        /// </summary>
        public int IncreasedVolumeValue { get; set; }
        /// <summary>
        /// Значение понижения громкости
        /// </summary>
        public int LoweredVolumeValue { get; set; }
        /// <summary>
        /// Стандратное значение громкости
        /// </summary>
        public int StandartVolumeValue { get; set; }
        
        #endregion



    }
}
