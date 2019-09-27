namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using static Helpers.SystemVariables;

    /// <summary>
    /// Эпизод фильма
    /// </summary>
    [Serializable]
    public class Episode
    {
        public Episode()
        {
            Jumpers = new List<Jumper>();
            Addresses = new List<string>();
            LastDateViewed = AppVal.ResetTime;
        }
        /// <summary>
        /// Номер эпизода
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Номер сезона
        /// </summary>
        public int SeasonNumber { get; set; }
        /// <summary>
        /// Полный номер эпизода
        /// </summary>
        [JsonIgnore]
        public int FullNumber => SeasonNumber * 100 + Number;
        /// <summary>
        /// Название эпизода
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание эпизода
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Флаг выбора эпизода
        /// </summary>
        public bool Checked { get; set; } = true;
        /// <summary>
        /// Изображение преобразованное в массив байт
        /// </summary>
        public byte[] ImageBytes { get; set; }
        /// <summary>
        /// Выбранный адрес эпизода
        /// </summary>
        public string SelectedAddress { get; set; }
        /// <summary>
        /// Список возможных адресов эпизода
        /// </summary>
        public List<string> Addresses { get; set; }
        /// <summary>
        /// Номер предыдущего эпизода в дилогиях/трилогиях и т.д.
        /// </summary>
        public int PrevChainLink { get; set; }
        /// <summary>
        /// Номер следущего эпизода в дилогиях/трилогиях и т.д.
        /// </summary>
        public int NextChainLink { get; set; }
        /// <summary>
        /// Полная длительность фильма
        /// </summary>
        public TimeSpan TotalDuration { get; set; }
        /// <summary>
        /// Время окончания эпизода (начало титров)
        /// </summary>
        public TimeSpan EpisodeEndTime { get; set; }
        /// <summary>
        /// Фактическая длительность эпизода (включая джамперы(пропуски) и окончание эпизода)
        /// </summary>
        [JsonIgnore]
        public TimeSpan ActualDuration =>
            TotalDuration - TimeSpan.FromSeconds((int)Jumpers
                                                      .Where(j => j.JumperMode == JumperMode.Skip)
                                                      .Sum(j => j.Duration.TotalSeconds))
                          - (TotalDuration - EpisodeEndTime);
        /// <summary>
        /// Последняя дата просмотра эпизода
        /// </summary>
        public DateTime LastDateViewed { get; set; }
        /// <summary>
        /// Список джамперов
        /// </summary>
        [JsonIgnore]
        public List<Jumper> Jumpers { get; set; }
    }
}
