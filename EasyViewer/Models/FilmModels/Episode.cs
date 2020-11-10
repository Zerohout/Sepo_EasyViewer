namespace EasyViewer.Models.FilmModels
{
    using Helpers;
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Эпизод фильма
    /// </summary>
    [Serializable]
    public class Episode : IFilmEntity, ISeasonEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Номер эпизода
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Номер сезона
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int SeasonNumber => Season.Number;

        /// <summary>
        /// Полный номер эпизода
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int FullNumber => SeasonNumber * 100 + Number;

        [BsonIgnore] [JsonIgnore] public string NumberName => $"Эпизод №{Number}";

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


        public Film Film { get; set; }
        [BsonIgnore] [JsonIgnore] public int FilmId => Film.Id;

        public Season Season { get; set; }
        [BsonIgnore] [JsonIgnore] public int SeasonId => Season.Id;

        /// <summary>
        /// Выбранный адрес эпизода
        /// </summary>
        public AddressInfo AddressInfo { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public List<AddressInfo> AddressInfoList => DbMethods.GetAddressInfoListFromDbByEpisodeId(Id);

        [BsonIgnore]
        [JsonIgnore]
        public List<Jumper> Jumpers => new List<Jumper>(AddressInfoList.SelectMany(addressInfo => addressInfo.Jumpers));

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