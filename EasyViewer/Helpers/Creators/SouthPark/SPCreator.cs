namespace EasyViewer.Helpers.Creators.SouthPark
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Windows.Forms;
	using Models.FilmModels;
	using ViewModels;
	using Vlc.DotNet.Wpf;
	using static SystemVariables;
	using static SPOnline;
	using static SPFreehat;
	using static GlobalMethods;
	using static CreatorMethods;
	using Application = System.Windows.Application;

	public static class SPCreator
	{
		public static void CreateSP(WaitViewModel wvm)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				Vlc = new VlcControl();
				Vlc.SourceProvider.CreatePlayer(VlcDataPath);
			});

			var films = GetDbCollection<Film>();

			if (films.All(f => f.Name != SP))
			{
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
					FilmType = SystemVariables.FilmType.Мультсериал
				};

				AddFilmToDb(result);
			}


			var stopwatch = new Stopwatch();
			stopwatch.Start();
			
			CreateSPSeasons(GetFilmFromDb(SP), wvm, stopwatch);
			CreateSPEpisodes(GetFilmFromDb(SP), wvm, stopwatch);
			CreateSPAddresses(GetFilmFromDb(SP), wvm, stopwatch);
			AddSPAddressesDurations(GetFilmFromDb(SP), wvm, stopwatch);

			Vlc.Dispose();
		}



		/// <summary>
		/// Создать общий список сезонов в фильме (Южный Парк)
		/// </summary>
		/// <param name="film">Фильм (Южный Парк)</param>
		/// <param name="wvm">Индикатор прогресса загрузки</param>
		/// <param name="stopwatch">Таймер</param>
		private static void CreateSPSeasons(Film film, WaitViewModel wvm, Stopwatch stopwatch)
		{
			wvm.CurrentLoadingStatus = LoadingStatus.Create_Seasons;
			if (AddingFilmToken.IsCancellationRequested) return;
			var seasonsCount = 23;
			wvm.MaxSeasonValue = seasonsCount;
			wvm.MaxPercentValue = seasonsCount;

			for (var i = 0; i < seasonsCount; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return;
				var num = i + 1;
				wvm.CurrentSeasonValue++;
				wvm.CurrentPercentValue++;

				if (film.Seasons.Any(s => s.Number == num)) continue;

				film.Seasons.Add(new Season
				{
					Number = num,
					Description = $"Description of Season_{num}"
				});

				UpdateDbCollection(film);

				wvm.ElapsedTime = stopwatch.Elapsed;
				wvm.RemainingTime = stopwatch.Elapsed;
			}
		}

		/// <summary>
		/// Создать общий список эпизодов в каждом сезоне фильма (Южный Парк)
		/// </summary>
		/// <param name="film">Фильм (Южный Парк)</param>
		/// <param name="wvm">Индикатор прогресса загрузки</param>
		/// <param name="stopwatch">Таймер</param>
		private static void CreateSPEpisodes(Film film, WaitViewModel wvm, Stopwatch stopwatch)
		{
			wvm.CurrentLoadingStatus = LoadingStatus.Create_Episodes;
			if (AddingFilmToken.IsCancellationRequested) return;
			var seasonsCount = film.Seasons.Count;
			wvm.MaxPercentValue = seasonsCount;
			wvm.MaxSeasonValue = seasonsCount;

			for (var i = 0; i < seasonsCount; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return;
				var seasonNum = i + 1;
				var episodesCount = GetSPEpisodesCount(seasonNum);
				wvm.ResetCurrentLoadingData(true);
				wvm.MaxEpisodeValue = episodesCount;
				wvm.CurrentSeasonValue++;
				wvm.CurrentPercentValue++;

				for (var j = 0; j < episodesCount; j++)
				{
					if (AddingFilmToken.IsCancellationRequested) return;
					wvm.CurrentEpisodeValue++;
					var epNum = j + 1;
					var epFullNum = seasonNum * 100 + epNum;

					if (film.Episodes.Any(e => e.FullNumber == epFullNum)) continue;

					film.Seasons[i].Episodes.Add(new Episode
					{
						Name = $"Episode {epNum} of {seasonNum} season",
						Description = $"Description of {epNum} episode of {seasonNum} season",
						Number = epNum,
						SeasonNumber = seasonNum
					});

					UpdateDbCollection(film);

					wvm.ElapsedTime = stopwatch.Elapsed;
					wvm.RemainingTime = stopwatch.Elapsed;
				}
			}
		}

		/// <summary>
		/// Создать общий список адресов (без длительности) эпизодов в каждом сезоне фильма (Южный Парк)
		/// </summary>
		/// <param name="film">Фильм (Южный Парк)</param>
		/// <param name="wvm">Индикатор прогресса загрузки</param>
		/// <param name="stopwatch">Таймер</param>
		private static void CreateSPAddresses(Film film, WaitViewModel wvm, Stopwatch stopwatch)
		{
			wvm.CurrentLoadingStatus = LoadingStatus.Create_Addresses;
			if (AddingFilmToken.IsCancellationRequested) return;
			var seasonsCount = film.Seasons.Count;

			wvm.MaxPercentValue = seasonsCount;
			wvm.MaxSeasonValue = seasonsCount;
			

			for (var i = 0; i < seasonsCount; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return;
				var season = film.Seasons[i];
				var seasonNum = season.Number;
				var episodesCount = season.Episodes.Count;
				wvm.MaxEpisodeValue = episodesCount;
				wvm.CurrentSeasonValue++;
				wvm.ResetCurrentLoadingData(true, true);
				wvm.CurrentPercentValue++;

				for (var j = 0; j < episodesCount; j++)
				{
					if (AddingFilmToken.IsCancellationRequested) return;
					var episode = film.Seasons[i].Episodes[j];
					var epNum = episode.Number;
					var voiceCountSPOnline = GetSPEpVoicesCountSPOnline(seasonNum, episode.Number);
					var voiceCountSpFreehat = GetSPEpVoicesCountSPFreehat(seasonNum, episode.Number);
					var addressesCount = voiceCountSpFreehat + voiceCountSPOnline;

					wvm.MaxAddressNumber = addressesCount;
					wvm.CurrentEpisodeValue++;
					wvm.ResetCurrentLoadingData(false,true);

					for (var k = 0; k < voiceCountSPOnline; k++)
					{
						if (AddingFilmToken.IsCancellationRequested) return;
						wvm.CurrentAddressNumber++;

						var addressNum = k + 1;
						var address = GetSPEpAddressSPOnline(seasonNum, epNum, addressNum);
						if (episode.Addresses.Any(a => a.Address == address)) continue;

						var voice = GetSPEpVoiceNameSPOnline(seasonNum, addressNum);

						film.Seasons[i].Episodes[j].Addresses.Add(new EpAddress
						{
							Name = $"online-south-park.ru_{voice}",
							Address = address,
							VoiceOver = voice
						});

						UpdateDbCollection(film);
					}

					for (var k = 0; k < voiceCountSpFreehat; k++)
					{
						if (AddingFilmToken.IsCancellationRequested) return;
						wvm.CurrentAddressNumber++;

						var addressNum = k + 1;
						var address = GetSPEpAddressSPFreehat(seasonNum, epNum, addressNum);
						if (episode.Addresses.Any(a => a.Address == address)) continue;

						var voice = GetSPEpVoiceNameSPFreehat(seasonNum, addressNum);

						film.Seasons[i].Episodes[j].Addresses.Add(new EpAddress
						{
							Name = $"sp.freehat.cc_{voice}",
							Address = address,
							VoiceOver = voice
						});

						UpdateDbCollection(film);

						wvm.ElapsedTime = stopwatch.Elapsed;
						wvm.RemainingTime = stopwatch.Elapsed;
					}

					if (episode.Address is null)
					{
						var addresses = film.Seasons[i].Episodes[j].Addresses;
						film.Seasons[i].Episodes[j].Address = addresses.First();
						UpdateDbCollection(film);
					}
				}
			}
		}

		/// <summary>
		/// Добавить длительности эпизодов к каждому адресу
		/// </summary>
		/// <param name="film">Фильм (Южный Парк)</param>
		/// <param name="wvm">Индикатор прогресса загрузки</param>
		/// <param name="stopwatch">Таймер</param>
		private static void AddSPAddressesDurations(Film film, WaitViewModel wvm, Stopwatch stopwatch)
		{
			wvm.CurrentLoadingStatus = LoadingStatus.Add_Durations;
			if (AddingFilmToken.IsCancellationRequested) return;
			var seasons = film.Seasons;
			wvm.MaxPercentValue = seasons.Count;
			wvm.MaxSeasonValue = seasons.Count;
			
			for (var i = 0; i < seasons.Count; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return;
				var episodes = seasons[i].Episodes;
				wvm.MaxEpisodeValue = episodes.Count;
				wvm.CurrentSeasonValue++;
				wvm.ResetCurrentLoadingData(true, true);
				wvm.CurrentPercentValue++;
				
				for (var j = 0; j < episodes.Count; j++)
				{
					if (AddingFilmToken.IsCancellationRequested) return;
					var addresses = episodes[j].Addresses;
					wvm.MaxAddressNumber = addresses.Count;
					wvm.CurrentEpisodeValue++;
					wvm.ResetCurrentLoadingData(false, true);
					
					for (var k = 0; k < addresses.Count; k++)
					{
						if (AddingFilmToken.IsCancellationRequested) return;
						var address = addresses[k];
						wvm.CurrentAddressNumber++;

						if (address.TotalDuration > new TimeSpan()) continue;

						var duration = GetEpisodeDuration(film.Name, address.Address, episodes[j].FullNumber, wvm,
							stopwatch);

						film.Seasons[i].Episodes[j].Addresses[k].TotalDuration = duration;
						film.Seasons[i].Episodes[j].Addresses[k].FilmEndTime = duration;

						UpdateDbCollection(film);
						wvm.ElapsedTime = stopwatch.Elapsed;
						wvm.RemainingTime = stopwatch.Elapsed;
					}
				}
			}
		}
	}
}
