namespace EasyViewer.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using LiteDB;
    using Models.FilmModels;
    using Models.SettingModels;
    using static SystemVariables;

    public static class DbMethods
    {
        /// <summary>
        ///     Загрузить или создать при их отсутсвии настройки просмотра
        /// </summary>
        /// <returns></returns>
        public static WatchingSettings LoadOrCreateWatchingSettings()
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var genSet = db.GetCollection<WatchingSettings>();

                if (genSet.Count() < 1) genSet.Insert(new WatchingSettings());

                AppVal.WS = genSet.FindAll().Last();
                //AppVal.EpisodesCount = AppVal.WS.DefaultEpCount;
            }

            return AppVal.WS;
        }

        #region База данных

        #region Общие методы

        /// <summary>
        ///     Получить коллекцию из БД
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <returns></returns>
        public static List<T> GetDbCollection<T>()
        {
            using var db = new LiteDatabase(DBPath);
            return new List<T>(db.GetCollection<T>().FindAll());
        }

        /// <summary>
        ///     Добавить сущность в БД
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="entity">Сущность</param>
        public static void InsertEntityToDb<T>(T entity)
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().Insert(entity);
        }

        /// <summary>
        ///     Добавить список сущностей в БД
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="entityList">Список сущностей</param>
        public static void InsertEntityListToDb<T>(List<T> entityList)
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().Insert(entityList);
        }

        public static void InsureIndexToDbEntity<T>(string indexName)
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().DropIndex(indexName);
        }

        /// <summary>
        ///     Удалить сущность из БД
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="entityId">ID сущности</param>
        public static void DeleteEntityFromDb<T>(int entityId)
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().Delete(entityId);
        }

        /// <summary>
        ///     Удалить все сущности фильма (наследники IFilm) из БД
        /// </summary>
        /// <typeparam name="T">Тип сущностей</typeparam>
        /// <param name="film">Фильм сущностей</param>
        public static void DeleteFilmEntities<T>(Film film)
            where T : IFilmEntity
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().DeleteMany(entity => entity.Film.Id == film.Id);
        }

        /// <summary>
        ///     Удалить все сущности сезона (наследники ISeason) из БД
        /// </summary>
        /// <typeparam name="T">Тип сущностей</typeparam>
        /// <param name="season">Сезон сущностей</param>
        public static void DeleteSeasonEntities<T>(Season season)
            where T : ISeasonEntity
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().DeleteMany(entity => entity.Season.Id == season.Id);
        }

        /// <summary>
        ///     Удалить все сущности эпизода (наследники IEpisode) из БД
        /// </summary>
        /// <typeparam name="T">Тип сущностей</typeparam>
        /// <param name="episode">Эпизод сущностей</param>
        public static void DeleteEpisodeEntities<T>(Episode episode)
            where T : IEpisodeEntity
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().DeleteMany(entity => entity.Episode.Id == episode.Id);
        }

        /// <summary>
        ///     Удалить все сущности AddressInfo (Наследники IAddressInfo) из БД
        /// </summary>
        /// <typeparam name="T">Тип сущностей</typeparam>
        /// <param name="addressInfo">AddressInfo сущностей</param>
        public static void DeleteAddressInfoEntities<T>(AddressInfo addressInfo)
            where T : IAddressInfoEntity
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().DeleteMany(entity => entity.AddressInfo.Id == addressInfo.Id);
        }

        /// <summary>
        ///     Полное удаление фильма со всеми его объектами из БД
        /// </summary>
        /// <param name="film">Фильм</param>
        public static void FullyFilmDeleting(Film film)
        {
            DeleteFilmEntities<Jumper>(film);
            DeleteFilmEntities<AddressInfo>(film);
            DeleteFilmEntities<Episode>(film);
            DeleteFilmEntities<Season>(film);
            DeleteEntityFromDb<Film>(film.Id);
        }

        /// <summary>
        ///     Полное удаление сезона со всеми его объектами из БД
        /// </summary>
        /// <param name="film">Фильм</param>
        public static void FullySeasonDeleting(Season season)
        {
            DeleteSeasonEntities<Jumper>(season);
            DeleteSeasonEntities<AddressInfo>(season);
            DeleteSeasonEntities<Episode>(season);
            DeleteEntityFromDb<Season>(season.Id);
        }

        /// <summary>
        ///     Полное удаление эпизода со всеми его объектами из БД
        /// </summary>
        /// <param name="film">Фильм</param>
        public static void FullyEpisodeDeleting(Episode episode)
        {
            DeleteEpisodeEntities<Jumper>(episode);
            DeleteEpisodeEntities<AddressInfo>(episode);
            DeleteEntityFromDb<Episode>(episode.Id);
        }

        /// <summary>
        ///     Полное удаление AddressInfo со всеми его объектами из БД
        /// </summary>
        /// <param name="film">Фильм</param>
        public static void FullyAddressInfoDeleting(AddressInfo addressInfo)
        {
            DeleteAddressInfoEntities<Jumper>(addressInfo);
            DeleteEntityFromDb<AddressInfo>(addressInfo.Id);
        }


        /// <summary>
        ///     Обновить коллекцию в БД
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="objCollection">Коллекция с измененными данными</param>
        public static void UpdateDbCollection<T>(IEnumerable<T> objCollection)
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().Update(objCollection);
        }

        /// <summary>
        ///     Обновить сущность в БД
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="entity">Новая сущность</param>
        public static void UpdateDbCollection<T>(T entity)
        {
            using var db = new LiteDatabase(DBPath);
            db.GetCollection<T>().Update(entity);
        }

        /// <summary>
        ///     Удалить коллекцию из БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DropDbCollection<T>()
        {
            bool result;

            using (var db = new LiteDatabase(DBPath))
            {
                result = db.DropCollection(typeof(T).Name);
            }

            return result;
        }

        #endregion

        #region Фильмы

        /// <summary>
        ///     Получить фильм из БД по имени
        /// </summary>
        /// <param name="filmName">Название фильма</param>
        /// <returns></returns>
        public static Film GetFilmFromDbByName(string filmName)
        {
            return GetDbCollection<Film>().First(f => f.Name == filmName);
        }

        /// <summary>
        ///     Получить фильм из БД по ID
        /// </summary>
        /// <param name="filmId">ID фильма</param>
        /// <returns></returns>
        public static Film GetFilmFromDbById(int filmId)
        {
            return GetDbCollection<Film>().First(f => f.Id == filmId);
        }

        #endregion

        #region Сезоны

        /// <summary>
        ///     Получить сезон из БД
        /// </summary>
        /// <param name="filmId">Id фильма</param>
        /// <param name="number">Номер сезона</param>
        /// <returns></returns>
        public static Season GetSeasonFromDb(int filmId, int number)
        {
            return GetDbCollection<Season>().Find(s => s.FilmId == filmId && s.Number == number);
        }

        /// <summary>
        ///     Получить список сезонов из БД по фильму
        /// </summary>
        /// <param name="filmId">ID фильма</param>
        /// <returns></returns>
        public static List<Season> GetSeasonListFromDbByFilmId(int filmId)
        {
            return GetDbCollection<Season>().FindAll(s => s.FilmId == filmId);
        }

        #endregion

        #region Эпизоды

        /// <summary>
        ///     Получить эпизод из БД
        /// </summary>
        /// <param name="filmId">Id фильма</param>
        /// <param name="fullNumber">Полный номер эпизода</param>
        /// <returns></returns>
        public static Episode GetEpisodeFromDb(int filmId, int fullNumber)
        {
            return GetDbCollection<Episode>().Find(e => e.FilmId == filmId && e.FullNumber == fullNumber);
        }

        public static Episode GetEpisodeFromDbById(int episodeId)
        {
            return GetDbCollection<Episode>().Find(e => e.Id == episodeId);
        }

        /// <summary>
        ///     Получить список эпизодов из БД по ID фильма
        /// </summary>
        /// <param name="filmId"></param>
        /// <returns></returns>
        public static List<Episode> GetEpisodeListFromDbByFilmId(int filmId)
        {
            return GetDbCollection<Episode>().FindAll(e => e.FilmId == filmId);
        }

        /// <summary>
        ///     Получить список эпизодов по ID сезона
        /// </summary>
        /// <param name="seasonId">ID сезона</param>
        /// <returns></returns>
        public static List<Episode> GetEpisodeListFromDbBySeasonId(int seasonId)
        {
            return GetDbCollection<Episode>().FindAll(e => e.Season.Id == seasonId);
        }

        #endregion

        #region AddressInfo

        public static List<AddressInfo> GetAddressInfoListByFilmId(int filmId)
        {
            return GetDbCollection<AddressInfo>().FindAll(a => a.Film.Id == filmId);
        }

        /// <summary>
        ///     Получить EpisodeInfo из БД по ID эпизода
        /// </summary>
        /// <param name="episodeId">ID эпизода</param>
        /// <returns></returns>
        public static AddressInfo GetAddressInfoFromDbByEpisodeId(int episodeId)
        {
            return GetDbCollection<AddressInfo>().Find(a => a.EpisodeId == episodeId);
        }

        public static AddressInfo GetAddressInfoFromDbById(int addressInfoId)
        {
            return GetDbCollection<AddressInfo>().FirstOrDefault(a => a.Id == addressInfoId);
        }

        /// <summary>
        ///     Получить список EpisodeInfo из БД по ID эпизода
        /// </summary>
        /// <param name="episodeId">ID эпизода</param>
        /// <returns></returns>
        public static List<AddressInfo> GetAddressInfoListFromDbByEpisodeId(int episodeId)
        {
            return GetDbCollection<AddressInfo>().FindAll(a => a.Episode.Id == episodeId);
        }

        #endregion

        #region Джамперы

        /// <summary>
        ///     Получить Jumper из БД по ID AddressInfo
        /// </summary>
        /// <param name="addressInfoId">ID AddressInfo</param>
        /// <param name="jumperNumber">Номер Jumper</param>
        /// <returns></returns>
        public static Jumper GetJumperFromDbByAddressInfoId(int addressInfoId, int jumperNumber)
        {
            return GetDbCollection<Jumper>()
                .Find(j => j.AddressInfoId == addressInfoId && j.Number == jumperNumber);
        }

        /// <summary>
        ///     Получить список Jumper из БД по ID AddressInfo
        /// </summary>
        /// <param name="addressInfoId">ID AddressInfo</param>
        /// <returns></returns>
        public static List<Jumper> GetJumperListFromDbByAddressInfoId(int addressInfoId)
        {
            return GetDbCollection<Jumper>().FindAll(j => j.AddressInfoId == addressInfoId);
        }

        #endregion

        #endregion
    }
}