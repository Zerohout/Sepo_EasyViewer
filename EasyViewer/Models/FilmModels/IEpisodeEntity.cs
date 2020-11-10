namespace EasyViewer.Models.FilmModels
{
    using LiteDB;
    using Newtonsoft.Json;

    public interface IEpisodeEntity
    {
        Episode Episode { get; set; }
        [BsonIgnore] [JsonIgnore] int EpisodeId { get; }
    }
}