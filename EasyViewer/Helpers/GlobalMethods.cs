namespace EasyViewer.Helpers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using LiteDB;
    using Models.FilmModels;
    using Models.SettingModels;
    using Newtonsoft.Json;
    using static SystemVariables;

    public static class GlobalMethods
    {
        /// <summary>
        /// Загрузить или создать при их отсутсвии настройки просмотра
        /// </summary>
        /// <returns></returns>
        public static WatchingSettings LoadOrCreateWatchingSettings()
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var genSet = db.GetCollection<WatchingSettings>();

                if (genSet.Count() < 1)
                {
                    genSet.Insert(new WatchingSettings());
                }

                AppVal.WS = genSet.FindAll().Last();
                AppVal.EpisodesCount = AppVal.WS.DefaultEpCount;
            }

            return AppVal.WS;
        }

        /// <summary>
        /// Получить коллекцию из базы данных
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <returns></returns>
        public static List<T> GetDbCollection<T>()
        {
            List<T> result;

            using (var db = new LiteDatabase(DBPath))
            {
                var collection = db.GetCollection<T>();
                result = new List<T>(collection.FindAll());
            }

            return result;
        }

        public static void RemoveFilmFromDb(Film film)
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var films = db.GetCollection<Film>();
                films.Delete(f => f.Id == film.Id);
            }
        }

        public static void AddFilmToDb(Film film)
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var films = db.GetCollection<Film>();
                films.Insert(film);
            }
        }

        /// <summary>
        /// Обновить коллекцию в базе данных
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="objCollection">Коллекция с измененными данными</param>
        public static void UpdateDbCollection<T>(IEnumerable<T> objCollection)
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var collection = db.GetCollection<T>();
                collection.Update(objCollection);
            }
        }

        /// <summary>
        /// Обновить эпизод в базе данных
        /// </summary>
        /// <param name="ep"></param>
        public static void UpdateDbEpisode(Episode ep)
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var filmsDb = db.GetCollection<Film>();

                var films = filmsDb.FindAll().ToList();

                films.SelectMany(f => f.Episodes)
                   .First(e => e.SelectedAddress == ep.SelectedAddress)
                   .LastDateViewed = ep.LastDateViewed;

                filmsDb.Update(films);

            }
        }
        /// <summary>
        /// Сравнить два объекта
        /// </summary>
        /// <param name="obj1">Первый объект</param>
        /// <param name="obj2">Второй объект</param>
        /// <returns></returns>
        public static bool IsEquals(object obj1, object obj2)
        {
            //var first1 = obj1 as WatchingSettings;
            //var second1 = obj2 as WatchingSettings;
            var first = JsonConvert.SerializeObject(obj1);
            var second = JsonConvert.SerializeObject(obj2);


            return first.Equals(second);


        }

        /// <summary>
        /// Загрузка изоражения из массива байт
        /// </summary>
        /// <param name="imageData">Массив байт</param>
        /// <returns></returns>
        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        /// <summary>
        /// Преобразовать изображение в массив байт
        /// </summary>
        /// <param name="imageIn">Необходимое изображение</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Активация/Деактивация трея
        /// </summary>
        /// <param name="view">Сворачиваемое представление</param>
        public static void ActivateDeactivateTray(Window view)
        {
            if (Tray.Visible)
            {
                Tray.Visible = false;
                view.Show();
                view.Activate();
            }
            else
            {
                Tray.Visible = true;
                view.Hide();
            }
        }

        /// <summary>
        /// Обновить коллекцию в базе данных
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="obj">Объект с измененными данными</param>
        public static void UpdateDbCollection<T>(T obj)
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var collection = db.GetCollection<T>();
                collection.Update(obj);
            }
        }

        /// <summary>
        /// Перевод слова на латиницу
        /// </summary>
        /// <param name="source"> это входная строка для транслитерации </param>
        /// <returns>получаем строку после транслитерации</returns>
        public static string TranslitFileName(string source)
        {
            var result = "";

            foreach (var ch in source)
            {
                if (TranslitDictionary.TryGetValue(ch.ToString(), out var ss))
                {
                    result += ss;
                }
                else
                {
                    result += ch;
                }
            }
            return result;
        }
    }
}
