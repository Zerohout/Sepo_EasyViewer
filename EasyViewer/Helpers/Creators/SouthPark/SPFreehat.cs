﻿namespace EasyViewer.Helpers.Creators.SouthPark
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Runtime.Remoting;
	using Models.FilmModels;
	using ViewModels;
	using static CreatorMethods;
	using static SystemVariables;

	public static class SPFreehat
	{
		private const string mtv = "mtv";
		private const string rentv = "rentv";
		private const string par = "par";
		private const string gob = "goblin";
		private const string vo = "vo";
		private const string kvk = "kvk";
		private const string ld = "ld";
		private const string js = "js";

		/// <summary>
		/// Получить список адресов эпизодов фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <param name="episodeNum">Номер эпизода</param>
		/// <returns></returns>
		public static List<EpAddress> GetSPEpAddressesSPFreehat(int seasonNum, int episodeNum, WaitViewModel wvm, Stopwatch stopwatch)
		{
			Uri address;
			TimeSpan duration;

			var result = new List<EpAddress>();
			var voicesCount = GetSPEpVoicesCountSPFreehat(seasonNum, episodeNum);

			for (var i = 1; i <= voicesCount; i++)
			{
				if (AddingFilmToken.IsCancellationRequested) return null;

				var num = i;
				var fullNum = seasonNum * 100 + episodeNum;
				wvm.CurrentAddressNumber++;
				var voice = GetSPEpVoiceNameSPFreehat(seasonNum, num);
				if (voice == null) continue;
				switch (seasonNum)
				{
					case 1:
						switch (voice)
						{
							case par:
								address = new Uri($"https://free.freehat.cc/sppar/free/{fullNum}_{voice}.mp4");
								break;
							default:
								address = new Uri($"https://free.freehat.cc/sp/free/{fullNum}_{voice}.mp4");
								break;
						}
						break;
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
					case 22:
						address = new Uri($"https://serv1.freehat.cc/cdn_yripmaq/sp/{fullNum}/{fullNum}_{voice}.m3u8");
						break;
					default:
						switch (voice)
						{
							case par:
								address = new Uri($"https://serv1.freehat.cc/cdn_yripmaq/sppar/{fullNum}/{fullNum}_{voice}.m3u8");
								break;
							default:
								address = new Uri($"https://serv1.freehat.cc/cdn_yripmaq/sp/{fullNum}/{fullNum}_{voice}.m3u8");
								break;
						}
						break;
				}
				duration = GetEpisodeDuration(SP, address, $"{seasonNum}{episodeNum}", wvm, stopwatch);
				wvm.RemainingTime = stopwatch.Elapsed;
				wvm.ElapsedTime = stopwatch.Elapsed;
				result.Add(new EpAddress
				{
					Name = $"sp.freehat.cc_{voice}",
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
		public static int GetSPEpVoicesCountSPFreehat(int seasonNum, int episodeNum)
		{
			switch (seasonNum)
			{
				case 2:
					switch (episodeNum)
					{
						case 1:
							return 2;
						default:
							return 3;
					}
				case 7:
					switch (episodeNum)
					{
						case 3:
							return 1;
						default:
							return 2;
					}

				case 8:
					switch (episodeNum)
					{
						case 1:
						case 4:
						case 5:
						case 6:
						case 8:
						case 9:
							return 1;
						default:
							return 2;
					}
				case 9:
					switch (episodeNum)
					{
						case 14:
							return 1;
						default:
							return 2;
					}
				case 10:
					switch (episodeNum)
					{
						case 3:
						case 4:
							return 1;
						default:
							return 2;
					}
				case 11:
					switch (episodeNum)
					{
						case 11:
							return 1;
						default:
							return 2;
					}
				case 12:
					switch (episodeNum)
					{
						case 4:
						case 6:
						case 8:
						case 9:
							return 1;
						default:
							return 2;
					}
				case 13:
					switch (episodeNum)
					{
						case 5:
							return 1;
						default:
							return 2;
					}
				case 14:
					switch (episodeNum)
					{
						case 3:
						case 5:
						case 6:
							return 1;
						default:
							return 2;
					}
				case 22:
					switch (episodeNum)
					{
						case 4:
						case 10:
							return 2;
						default:
							return 3;
					}
				case 3:
				case 4:
					return 1;
				case 5:
				case 6:
				case 16:
				case 17:
				case 18:
					return 2;
				default:
					return 3;
			}
		}

		/// <summary>
		/// Получить название озвучки сезона фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <param name="voiceNum">Номер озвучки</param>
		/// <returns></returns>
		private static string GetSPEpVoiceNameSPFreehat(int seasonNum, int episodeNum, int voiceNum = 1)
		{
			switch (seasonNum)
			{
				case 1:
					switch (voiceNum)
					{
						case 1:
							return mtv;
						case 2:
							return par;
						default:
							return gob;
					}
				case 2:
					switch (voiceNum)
					{
						case 1:
							return mtv;
						case 2:
							switch (episodeNum)
							{
								case 1:
									return gob;
								default:
									return par;
							}
						default:
							return gob;
					}
				case 3:
					switch (episodeNum)
					{
						case 2:
							return rentv;
						default:
							return mtv;
					}
				case 4:
					return "mtv";
				case 8:
					switch (voiceNum)
					{
						case 1:
							switch (episodeNum)
							{
								case 4:
									return vo;
								default:
									return mtv;
							}
						default:
							return par;

					}
				case 9:
					switch (voiceNum)
					{
						case 1:
							switch (episodeNum)
							{
								case 14:
									return vo;
								default:
									return mtv;
							}
						default:
							return par;
					}
				case 14:
					switch (voiceNum)
					{
						case 1:
							switch (episodeNum)
							{
								case 5:
								case 6:
									return kvk;
								default:
									return mtv;
							}
						default:
							return par;
					}
				case 15:
					switch (voiceNum)
					{
						case 1:
							return ld;
						case 2:
							return par;
						default:
							return mtv;
					}
				case 16:
					switch (voiceNum)
					{
						case 1:
							return ld;
						default:
							return mtv;
					}
				case 17:
				case 18:
					switch (voiceNum)
					{
						case 1:
							return ld;
						default:
							return par;
					}
				case 19:
				case 20:
				case 21:
					switch (voiceNum)
					{
						case 1:
							return ld;
						case 2:
							return par;
						default:
							return js;
					}
				case 22:
					switch (voiceNum)
					{
						case 1:
							return ld;
						case 2:
							switch (episodeNum)
							{
								case 4:
								case 10:
									return js;
								default:
									return par;
							}
						default:
							return js;
					}
				default:
					switch (voiceNum)
					{
						case 1:
							return mtv;
						default:
							return par;
					}
			}
		}
	}
}
