namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using LiteDB;
    using Newtonsoft.Json;

    /// <summary>
    ///     Сезон фильма
    /// </summary>
    [Serializable]
    public class Season : IFilmEntity
    {
        public int Id { get; set; }

        /// <summary>
        ///     Номер сезона
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Название сезона
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public string Name => $"Сезон №{Number}";

        /// <summary>
        ///     Описание сезона
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Флаг выбора сезона
        /// </summary>
        public bool Checked { get; set; } = true;

        public Film Film { get; set; }
        
        [BsonIgnore] [JsonIgnore] public int FilmId => Film.Id;

        [BsonIgnore] [JsonIgnore] public List<Episode> Episodes => DbMethods.GetEpisodeListFromDbBySeasonId(Id);
        [BsonIgnore] [JsonIgnore] public List<AddressInfo> AddressInfoList => new List<AddressInfo>(Episodes.SelectMany(episode => episode.AddressInfoList));
        [BsonIgnore] [JsonIgnore] public List<Jumper> Jumpers => new List<Jumper>(AddressInfoList.SelectMany(addressInfo => addressInfo.Jumpers));
    }
}