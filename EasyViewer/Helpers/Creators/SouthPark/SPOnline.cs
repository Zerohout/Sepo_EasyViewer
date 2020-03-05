namespace EasyViewer.Helpers.Creators.SouthPark
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Models.FilmModels;
	using ViewModels;
	using static CreatorMethods;
	using static SystemVariables;

	public static class SPOnline
	{
		private const string kub = "kubik";
		private const string par = "paramount";
		private const string mtv = "mtv";
		private const string gob = "goblin";

		/// <summary>
		/// Получить список адресов эпизодов фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <param name="episodeNum">Номер эпизода</param>
		/// <returns></returns>
		public static List<EpAddress> GetSPEpAddressesSPOnline(int seasonNum, int episodeNum, WaitViewModel wvm, Stopwatch stopwatch)
		{
			Uri address;
			TimeSpan duration;

			if (AddingFilmToken.IsCancellationRequested) return null;

			if (seasonNum == 15 &&
				episodeNum == 3)
			{
				address = new Uri("https://serv1.freehat.cc/cdn_yripmaq/sppar/1503/1503_par.m3u8");
				duration = GetEpisodeDuration(SP, address, $"{seasonNum}{episodeNum}", wvm, stopwatch);
				wvm.RemainingTime = stopwatch.Elapsed;
				wvm.ElapsedTime = stopwatch.Elapsed;
				wvm.CurrentAddressNumber++;

				return new List<EpAddress>
				{ new EpAddress
				{
					Name = "freehat.cc_paramount",
					VoiceOver = par,
					Address = address,
					TotalDuration = duration,
					FilmEndTime = duration
				}  };
			}

			var result = new List<EpAddress>();
			var ip = GetSPSeasonIpSPOnline(seasonNum);
			var voicesCount = GetSPEpVoicesCountSPOnline(seasonNum, episodeNum);

			for (var i = 1; i <= voicesCount; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return null;
				var num = i;
				var voice = GetSPEpVoiceNameSPOnline(seasonNum, num);
				wvm.CurrentAddressNumber++;
				if (voice == null) continue;
				address = new Uri(
					$"http://{ip}/video/spark/s{seasonNum:00}-{voice}/{GetSPEpAddressNumSPOnline(seasonNum, episodeNum, voice)}.mp4");
				duration = GetEpisodeDuration(SP, address, $"{seasonNum}{episodeNum}", wvm, stopwatch);
				wvm.RemainingTime = stopwatch.Elapsed;
				wvm.ElapsedTime = stopwatch.Elapsed;
				result.Add(new EpAddress
				{
					Name = $"online-south-park.ru_{voice}",
					VoiceOver = voice,
					Address = address,
					TotalDuration = duration,
					FilmEndTime = duration
				});
			}

			return result;
		}

		/// <summary>
		/// Получить количество озвучек эпизода фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <param name="episodeNum">Номер эпизода</param>
		/// <returns></returns>
		public static int GetSPEpVoicesCountSPOnline(int seasonNum, int episodeNum)
		{
			switch (seasonNum)
			{
				case 1:
				case 2:
				case 16:
				case 17:
				case 18:
				case 20:
				case 21:
					return 2;
				case 14:
					switch (episodeNum)
					{
						case 5:
						case 6:
							return 1;
						default:
							return 2;
					}
				case 15:
					switch (episodeNum)
					{
						case 3:
							return 0;
						default:
							return 2;
					}
				case 19:
					switch (episodeNum)
					{
						case 8:
						case 9:
						case 10:
							return 1;
						default:
							return 2;
					}
				case 22:
					switch (episodeNum)
					{
						case 4:
						case 10:
							return 1;
						default:
							return 2;
					}

				default:
					return 1;
			}
		}

		/// <summary>
		/// Получить название озвучки сезона фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <param name="voiceNum">Номер озвучки</param>
		/// <returns></returns>
		private static string GetSPEpVoiceNameSPOnline(int seasonNum, int voiceNum = 1)
		{
			switch (seasonNum)
			{
				case 1:
					switch (voiceNum)
					{
						case 1:
							return par;
						default:
							return gob;
					}
				case 2:
					switch (voiceNum)
					{
						case 1:
							return mtv;
						default:
							return gob;
					}
				case 14:
					switch (voiceNum)
					{
						case 1:
							return kub;
						default:
							return mtv;
					}
				case 10:
					return par;
				case 19:
					switch (voiceNum)
					{
						case 1:
							return par;
						default:
							return kub;
					}
				case 15:
				case 16:
				case 17:
				case 18:
				case 20:
				case 21:
				case 22:
					switch (voiceNum)
					{
						case 1:
							return kub;
						default:
							return par;
					}
				default:
					return mtv;
			}
		}

		/// <summary>
		/// Получить номер эпизода фильма Южный Парк для адреса
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <param name="episodeNum">Номер эпизода</param>
		/// <param name="voice">Название озвучки</param>
		/// <returns></returns>
		private static string GetSPEpAddressNumSPOnline(int seasonNum, int episodeNum, string voice)
		{
			switch (voice)
			{
				case gob:
					switch (seasonNum)
					{
						case 1:
							return $"{seasonNum}{episodeNum:00}";
						default:
							return $"{episodeNum:00}";
					}
				case mtv:
					switch (seasonNum)
					{
						case 9:
							return $"{seasonNum}{episodeNum:00}";
						case 14:
							switch (episodeNum)
							{
								case 5:
								case 6:
									return null;
								default:
									return $"{episodeNum:00}";
							}
						default:
							return $"{episodeNum:00}";
					}
				case "paramount":
					switch (seasonNum)
					{
						case 15:
							switch (episodeNum)
							{
								case 3:
									return null;
								default:
									return $"{seasonNum}{episodeNum:00}";
							}
						case 22:
							switch (episodeNum)
							{
								case 4:
									return null;
								default:
									return $"{episodeNum:00}";
							}
						default:
							return $"{episodeNum:00}";
					}
				case kub:
					switch (seasonNum)
					{
						case 15:
							switch (episodeNum)
							{
								case 3:
									return "";
								default:
									return $"{seasonNum}{episodeNum:00}";
							}
						case 17:
							return $"{seasonNum}{episodeNum:00}";
						default:
							return $"{episodeNum:00}";
					}
				default:
					return "";
			}
		}

		/// <summary>
		/// Получить ip-адрес сезона фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <returns></returns>
		private static string GetSPSeasonIpSPOnline(int seasonNum)
		{
			switch (seasonNum)
			{
				case 1:
				case 2:
				case 3:
				case 4:
					return "195.154.240.169";
				case 20:
				case 21:
				case 22:
					return "195.154.240.169";
				default:
					return "89.163.225.137";
			}
		}
	}
}
