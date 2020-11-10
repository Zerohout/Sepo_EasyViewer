namespace EasyViewer.Models.FilmModels
{
    using LiteDB;
    using Newtonsoft.Json;

    public interface IAddressInfoEntity
    {
        AddressInfo AddressInfo { get; set; }

        [BsonIgnore] [JsonIgnore] int AddressInfoId { get; }
    }
}