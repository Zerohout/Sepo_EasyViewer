namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
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
            Addresses = new List<EpAddress>();
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
        /// Выбранный адрес эпизода
        /// </summary>
        public EpAddress Address { get; set; }
        /// <summary>
        /// Список возможных адресов эпизода
        /// </summary>
        public List<EpAddress> Addresses { get; set; }
        /// <summary>
        /// Номер предыдущего эпизода в дилогиях/трилогиях и т.д.
        /// </summary>
        public int PrevChainLink { get; set; }
        /// <summary>
        /// Номер следущего эпизода в дилогиях/трилогиях и т.д.
        /// </summary>
        public int NextChainLink { get; set; }

        /// <summary>
        /// Последняя дата просмотра эпизода
        /// </summary>
        public DateTime LastDateViewed { get; set; }

    }
}
