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

		/// <summary>
		/// Получить фильм из бд
		/// </summary>
		/// <param name="filmName">Название необходимого фильма</param>
		/// <returns></returns>
		public static Film GetFilmFromDb(string filmName) =>
			GetDbCollection<Film>().First(f => f.Name == filmName);
		/// <summary>
		/// Получить сезон из бд
		/// </summary>
		/// <param name="filmName">Название необходимого фильма</param>
		/// <param name="number">Номер необходимого сезона</param>
		/// <returns></returns>
		public static Season GetSeasonFromDb(string filmName, int number) =>
			GetFilmFromDb(filmName).Seasons.First(s => s.Number == number);
		/// <summary>
		/// Получить эпизод из бд
		/// </summary>
		/// <param name="filmName">Название необходимого фильма</param>
		/// <param name="fullNumber">Полный номер необходимого эпизода</param>
		/// <returns></returns>
		public static Episode GetEpisodeFromDb(string filmName, int fullNumber) =>
			GetFilmFromDb(filmName).Episodes.First(e => e.FullNumber == fullNumber);

		/// <summary>
		/// Удалить фильм из бд
		/// </summary>
		/// <param name="film"></param>
		public static void RemoveFilmFromDb(Film film)
		{
			using (var db = new LiteDatabase(DBPath))
			{
				var films = db.GetCollection<Film>();
				films.Delete(f => f.Id == film.Id);
			}
		}

		/// <summary>
		/// Добавить фильм в бд
		/// </summary>
		/// <param name="film"></param>
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
		/// Обновит фильм в бд
		/// </summary>
		/// <param name="film"></param>
		public static void UpdateDbFilm(Film film) => UpdateDbCollection(obj: film);

		/// <summary>
		/// Обновит сезон в бд
		/// </summary>
		/// <param name="filmName"></param>
		/// <param name="season"></param>
		public static void UpdateDbSeason(string filmName, Season season)
		{
			var film = GetDbCollection<Film>()
				.Single(f => f.Name == filmName);
			var index = film.Seasons.FindIndex(s => s.Number == season.Number);
			film.Seasons[index] = season;
			UpdateDbFilm(film);

		}

		/// <summary>
		/// Обновить эпизод в бд
		/// </summary>
		/// <param name="episode"></param>
		public static void UpdateDbEpisode(Episode episode)
		{
			var film = GetDbCollection<Film>().Single(f => f.Episodes.Any(e => e.Address == episode.Address));
			var index = film.Seasons.First(s => s.Number == episode.SeasonNumber).Episodes.FindIndex(e => e.FullNumber == episode.FullNumber);
			film.Seasons.First(s => s.Number == episode.SeasonNumber).Episodes[index] = episode;
			UpdateDbFilm(film);
		}
		
		/// <summary>
		/// Сравнить два объекта
		/// </summary>
		/// <param name="obj1">Первый объект</param>
		/// <param name="obj2">Второй объект</param>
		/// <returns></returns>
		public static bool IsEquals(object obj1, object obj2)
		{
			var first = JsonConvert.SerializeObject(obj1);
			var second = JsonConvert.SerializeObject(obj2);
			return first.Equals(second);
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
		/// Перевод слова на латиницу
		/// </summary>
		/// <param name="source"> это входная строка для транслитерации </param>
		/// <returns>получаем строку после транслитерации</returns>
		public static string TranslateFileName(string source)
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
