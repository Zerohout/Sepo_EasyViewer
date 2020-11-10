namespace EasyViewer.Models.FilmModels
{
    using System;
    using LiteDB;
    using Newtonsoft.Json;
    using static Helpers.SystemVariables;

    /// <summary>
    ///     Джампер (выполнение операции на определенном отрезке времени)
    /// </summary>
    [Serializable]
    public class Jumper : IFilmEntity, ISeasonEntity, IEpisodeEntity, IAddressInfoEntity
    {

        public int Id { get; set; }
        public Film Film { get; set; }

        [BsonIgnore] [JsonIgnore] public int FilmId => Film.Id;

        public Season Season { get; set; }
        [BsonIgnore] [JsonIgnore] public int SeasonId => Season.Id;

        public Episode Episode { get; set; }

        [BsonIgnore] [JsonIgnore] public int EpisodeId => Episode.Id;
        public AddressInfo AddressInfo { get; set; }

        [BsonIgnore] [JsonIgnore] public int AddressInfoId => AddressInfo.Id;

        /// <summary>
        ///     Номер джампера в последовательности
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Время начала джампера
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        ///     Время окончания джампера
        /// </summary>
        public TimeSpan EndTime { get; set; }

        /// <summary>
        ///     Длительность джампера
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        ///     Режим джампера
        /// </summary>
        public JumperMode JumperMode { get; set; }

        /// <summary>
        ///     Флаг статуса работы джампера
        /// </summary>
        public bool IsWorking { get; set; }

        #region Управление звуком

        /// <summary>
        ///     Значение повышения громкости
        /// </summary>
        public int IncreasedVolumeValue { get; set; }

        /// <summary>
        ///     Значение понижения громкости
        /// </summary>
        public int LoweredVolumeValue { get; set; }

        /// <summary>
        ///     Стандратное значение громкости
        /// </summary>
        public int StandardVolumeValue { get; set; }

        #endregion
    }
}