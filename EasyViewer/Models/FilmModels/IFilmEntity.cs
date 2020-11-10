namespace EasyViewer.Models.FilmModels
{
    using LiteDB;
    using Newtonsoft.Json;

    public interface IFilmEntity
    {
        Film Film { get; set; }

        [BsonIgnore] [JsonIgnore] int FilmId { get; }
    }
}