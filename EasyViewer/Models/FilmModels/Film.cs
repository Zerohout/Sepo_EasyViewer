using System.Linq;

namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Helpers;
    using static Helpers.SystemVariables;

    /// <summary>
    /// фильм (мультфильм, кинофильм, видео и т.д.)
    /// </summary>
    [Serializable]
    public class Film
    {
        /// <summary>
        /// ID фильма
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название фильма
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание фильма
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Флаг выбора фильма
        /// </summary>
        public bool Checked { get; set; } = true;

        /// <summary>
        /// Тип фильма
        /// </summary>
        public FilmType FilmType { get; set; }

        [LiteDB.BsonIgnore] 
        [JsonIgnore] 
        public List<Season> Seasons => DbMethods.GetSeasonListFromDbByFilmId(Id);

        [LiteDB.BsonIgnore]
        [JsonIgnore]
        public List<Episode> Episodes => DbMethods.GetEpisodeListFromDbByFilmId(Id);

        [LiteDB.BsonIgnore]
        [JsonIgnore]
        public List<AddressInfo> AddressInfoList => DbMethods.GetAddressInfoListByFilmId(Id);

        [LiteDB.BsonIgnore]
        [JsonIgnore]
        public List<Jumper> Jumpers => new List<Jumper>(AddressInfoList.SelectMany(addressInfo => addressInfo.Jumpers));

        [LiteDB.BsonIgnore]
        [JsonIgnore]
        public List<Episode> CheckedEpisodes => Checked
            ? Episodes.FindAll(episode => episode.Season.Checked && episode.Checked &&
                                          DateTime.Now.Subtract(episode.LastDateViewed).TotalDays >
                                          (AppVal.WS.NonRepeatDaysInterval ?? -1))
            : new List<Episode>();
    }
}