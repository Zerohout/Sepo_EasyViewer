namespace EasyViewer.Helpers.Creators.SouthPark
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Windows;
	using Models.FilmModels;
	using ViewModels;
	using Vlc.DotNet.Wpf;
	using static SystemVariables;
	using static SPOnline;
	using static SPFreehat;
	using static GlobalMethods;

	public static class SPCreator
	{
		/// <summary>
		/// Создать фильм Южный Парк
		/// </summary>
		/// <param name="wvm">Представление окна ожидания</param>
		/// <returns></returns>
		public static Film CreateSP(WaitViewModel wvm)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				Vlc = new VlcControl();
				Vlc.SourceProvider.CreatePlayer(VlcDataPath);
			});

			var result = new Film
			{
				Name = SP,
				Description = "«Южный парк» (он же South Park и Саус Парк) — фильм картинками, " +
							  "который отличается от других мультиков тем, что градус треша и угара в нем зашкаливает, " +
							  "сферически воплощаясь в каждом из жителей маленького и нифига не тихого городка в Колорадо, " +
							  "которые окружают главных героев: Эрик Картман — маленький расист с зашкаливающим чувством величия " +
							  "и неуемной жаждой действий. Стэн Марш — обладатель критичного ума, офигевающий над своим папой " +
							  "и креативным долбозвоном в одном лице, Рэнди Маршем. Кайл Брофловски — рыжеволосый и ироничный " +
							  "персонаж, чье здравомыслие с лихвой уравнивает общую шизофрению происходящего (постоянный предмет " +
							  "нападок жирдяя) и главный убиванец фильма, Кенни МакКормик — бичеватый индивид с капюшоном на " +
							  "все лицо и пошловатой логикой микроскопического эротомана. Не менее колоритен ныне заменивший " +
							  "вечно подыхающего коротыша другой мегаломаньяк — Профессор Хаос, ненавистник пиписек и " +
							  "латентный гомосексуалист — Баттерс Стотч.",
				FilmType = FilmType.Мультсериал,
				Seasons = new List<Season>(CreateSPSeasons(wvm, SP) ?? new List<Season>())
			};

			Vlc.Dispose();
			return result;
		}

		/// <summary>
		/// Создать список сезонов фильма Южный парк
		/// </summary>
		/// <param name="wvm"></param>
		/// <returns></returns>
		private static List<Season> CreateSPSeasons(WaitViewModel wvm, string filmName)
		{
			wvm.MaximumValue = 297;
			var result = new List<Season>();
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			for (var i = 1; i <= 22; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return null;
				var num = i;
				result.Add(new Season
				{
					Number = num,
					Description = $"Description of Season_{num}",
					Episodes = new List<Episode>(CreateSPEpisodes(wvm, num, stopWatch, filmName) ?? new List<Episode>())
				});
			}

			return result;
		}

		/// <summary>
		/// Создать список эпизодов фильма Южный Парк
		/// </summary>
		/// <param name="wvm">Представление окна ожидания</param>
		/// <param name="seasonNum">Номер эпизода</param>
		/// <param name="stopwatch">Таймер для окна ожидания</param>
		/// <returns></returns>
		private static List<Episode> CreateSPEpisodes(WaitViewModel wvm, int seasonNum, Stopwatch stopwatch, string filmName)
		{
			var result = new List<Episode>();
			var episodesCount = GetSPEpisodesCount(seasonNum);

			for (var i = 0; i < episodesCount; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return null;
				var num = i + 1;
				wvm.CurrentValue++;
				var addressesValue = GetSPEpVoicesCountSPOnline(seasonNum, num) +
									 GetSPEpVoicesCountSPFreehat(seasonNum, num);
				wvm.MaxAddressNumber = addressesValue;
				var addresses = new List<EpAddress>(GetSPEpAddressesSPOnline(seasonNum, num, wvm, stopwatch) ?? new List<EpAddress>());
				if (AddingFilmToken.IsCancellationRequested) return null;

				addresses.AddRange(GetSPEpAddressesSPFreehat(seasonNum, num, wvm, stopwatch) ?? new List<EpAddress>());
				if (AddingFilmToken.IsCancellationRequested) return null;
				var fullNum = seasonNum * 100 + num;
				var name = $"{TranslateFileName(filmName)}_S{seasonNum}_E{num}";
				result.Add(new Episode
				{
					Name = name,
					Number = num,
					SeasonNumber = seasonNum,
					Description = $"Description of {name}",
					Addresses = new List<EpAddress>(addresses),
					Address = addresses.First(),
					PrevChainLink = GetPrevSPChainEpNum(fullNum),
					NextChainLink = GetNextSPChainEpNum(fullNum),
					Checked = true,
				});
				wvm.CurrentAddressNumber = 0;
			}

			return result;
		}

		/// <summary>
		/// Получить номер предыдущего эпизода в цепи эпизодов
		/// </summary>
		/// <param name="epFullNum">Полный номер эпизода</param>
		/// <returns></returns>
		private static int GetPrevSPChainEpNum(int epFullNum)
		{
			var season = epFullNum / 100;
			var episode = epFullNum % 100;

			switch (season)
			{
				case 2:
					switch (episode)
					{
						case 2:
							return 113;
						default:
							return 0;
					}
				case 4:
					switch (episode)
					{
						case 11:
							return 410;
						default:
							return 0;
					}
				case 6:
					switch (episode)
					{
						case 7:
							return 606;
						default:
							return 0;
					}
				case 10:
					switch (episode)
					{
						case 4:
							return 1003;
						case 13:
							return 1012;
						default:
							return 0;
					}
				case 11:
					switch (episode)
					{
						case 11:
							return 1110;
						case 12:
							return 1111;
						default:
							return 0;
					}
				case 12:
					switch (episode)
					{
						case 11:
							return 1210;
						default:
							return 0;
					}
				case 14:
					switch (episode)
					{
						case 6:
							return 1405;
						case 12:
							return 1411;
						case 13:
							return 1412;
						default:
							return 0;
					}
				case 15:
					switch (episode)
					{
						case 8:
							return 1507;
						default:
							return 0;
					}
				case 17:
					switch (episode)
					{
						case 8:
							return 1707;
						case 9:
							return 1708;
						default:
							return 0;
					}
				case 18:
					switch (episode)
					{
						case 2:
							return 1801;
						case 10:
							return 1809;
						default:
							return 0;
					}
				case 19:
					switch (episode)
					{
						case 9:
							return 1908;
						case 10:
							return 1909;
						default:
							return 0;
					}
				case 22:
					switch (episode)
					{
						case 7:
							return 2206;
						case 10:
							return 2209;
						default:
							return 0;
					}
				default:
					return 0;
			}
		}

		/// <summary>
		/// Получить номер следующего эпизода в цепи эпизодов
		/// </summary>
		/// <param name="epFullNum">Полный номер эпизода</param>
		/// <returns></returns>
		private static int GetNextSPChainEpNum(int epFullNum)
		{
			var season = epFullNum / 100;
			var episode = epFullNum % 100;

			switch (season)
			{
				case 1:
					switch (episode)
					{
						case 13:
							return 202;
						default:
							return 0;
					}
				case 4:
					switch (episode)
					{
						case 10:
							return 411;
						default:
							return 0;
					}
				case 6:
					switch (episode)
					{
						case 6:
							return 607;
						default:
							return 0;
					}
				case 10:
					switch (episode)
					{
						case 3:
							return 1004;
						case 12:
							return 1013;
						default:
							return 0;
					}
				case 11:
					switch (episode)
					{
						case 10:
							return 1111;
						case 11:
							return 1112;
						default:
							return 0;
					}
				case 12:
					switch (episode)
					{
						case 10:
							return 1211;
						default:
							return 0;
					}
				case 14:
					switch (episode)
					{
						case 5:
							return 1406;
						case 11:
							return 1412;
						case 12:
							return 1413;
						default:
							return 0;
					}
				case 15:
					switch (episode)
					{
						case 7:
							return 1508;
						default:
							return 0;
					}
				case 17:
					switch (episode)
					{
						case 7:
							return 1708;
						case 8:
							return 1709;
						default:
							return 0;
					}
				case 18:
					switch (episode)
					{
						case 1:
							return 1802;
						case 9:
							return 1810;
						default:
							return 0;
					}
				case 19:
					switch (episode)
					{
						case 8:
							return 1909;
						case 9:
							return 1910;
						default:
							return 0;
					}
				case 22:
					switch (episode)
					{
						case 6:
							return 2207;
						case 9:
							return 2210;
						default:
							return 0;
					}
				default:
					return 0;
			}
		}

		/// <summary>
		/// Получить количество эпизодов в сезоне фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <returns></returns>
		private static int GetSPEpisodesCount(int seasonNum)
		{
			switch (seasonNum)
			{
				case 1:
					return 13;
				case 2:
					return 18;
				case 3:
				case 4:
				case 6:
					return 17;
				case 7:
					return 15;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
					return 10;
				default:
					return 14;
			}
		}

	}
}
