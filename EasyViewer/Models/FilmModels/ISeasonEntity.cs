namespace EasyViewer.Models.FilmModels
{
    using LiteDB;
    using Newtonsoft.Json;

    public interface ISeasonEntity
    {
        Season Season { get; set; }
        [BsonIgnore] [JsonIgnore] int SeasonId { get; }
    }
}