namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using LiteDB;
    using Newtonsoft.Json;

    [Serializable]
    public class AddressInfo : IFilmEntity, ISeasonEntity, IEpisodeEntity
    {
        public int Id { get; set; }

        /// <summary>
        ///     Название адреса
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///     Название озвучки
        /// </summary>
        public string VoiceOver { get; set; }

        /// <summary>
        ///     Адрес
        /// </summary>
        public Uri Link { get; set; }

        /// <summary>
        ///     Время окончания фильма (начало титров)
        /// </summary>
        public TimeSpan FilmEndTime { get; set; }

        /// <summary>
        ///     Полная длительность фильма
        /// </summary>
        public TimeSpan TotalDuration { get; set; }

        [JsonIgnore] [BsonIgnore] public List<Jumper> Jumpers => DbMethods.GetJumperListFromDbByAddressInfoId(Id);

        [JsonIgnore]
        [BsonIgnore]
        public TimeSpan AddressActualDuration = new TimeSpan();

        /// <summary>
        ///     Фактическая длительность эпизода (включая джамперы(пропуски) и окончание эпизода)
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public TimeSpan ActualDuration {
            get
            {
                if (AddressActualDuration > new TimeSpan()) return AddressActualDuration;

                return FilmEndTime - TimeSpan.FromSeconds((int) Jumpers
                    .Where(j => j.JumperMode == SystemVariables.JumperMode.Skip)
                    .Sum(j => j.Duration.TotalSeconds));
            }

        }
            

        public Episode Episode { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public bool Checked
        {
            get
            {
                var episode = DbMethods.GetEpisodeFromDbById(Episode.Id);
                return GlobalMethods.IsEquals(episode.AddressInfo, this);
            }
        }

        [BsonIgnore] [JsonIgnore] public int EpisodeId => Episode.Id;

        public Film Film { get; set; }

        [BsonIgnore] [JsonIgnore] public int FilmId => Film.Id;
        public Season Season { get; set; }
        [BsonIgnore] [JsonIgnore] public int SeasonId => Season.Id;
    }
}