namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using Newtonsoft.Json;

    [Serializable]
    public class EpAddress
    {
        public EpAddress()
        {
            Jumpers = new List<Jumper>();
        }
        /// <summary>
        /// Название адреса
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Название озвучки
        /// </summary>
        public string VoiceOver { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public Uri Address { get; set; }
        /// <summary>
        /// Время окончания фильма (начало титров)
        /// </summary>
        public TimeSpan FilmEndTime { get; set; }
        /// <summary>
        /// Полная длительность фильма
        /// </summary>
        public TimeSpan TotalDuration { get; set; }
        /// <summary>
        /// Фактическая длительность эпизода (включая джамперы(пропуски) и окончание эпизода)
        /// </summary>
        [JsonIgnore]
        public TimeSpan ActualDuration =>
            TotalDuration - TimeSpan.FromSeconds((int)Jumpers
                                                      .Where(j => j.JumperMode == SystemVariables.JumperMode.Skip)
                                                      .Sum(j => j.Duration.TotalSeconds))
                          - (TotalDuration - FilmEndTime);
        /// <summary>
        /// Список джамперов
        /// </summary>
        [JsonIgnore]
        public List<Jumper> Jumpers { get; set; }

    }
}
