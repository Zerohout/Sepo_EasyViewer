namespace EasyViewer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using Models.FilmModels;
    using ViewModels;
    using Vlc.DotNet.Wpf;
    using static GlobalMethods;
    using static SystemVariables;

    public static class SPCreator
    {
        private static Random rnd;
        private static byte[] imageBytes = ImageToByteArray(new Bitmap($"{AppDataPath}\\{DefaultLogoImageName}"));
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
                Name = "Южный парк",
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
                ImageBytes = imageBytes.ToArray(),
                FilmType = FilmType.Мультсериал,
                Seasons = new List<Season>(CreateSPSeasons(wvm))
            };

            Vlc.Dispose();
            return result;
        }

        /// <summary>
        /// Создать список сезонов фильма Южный парк
        /// </summary>
        /// <param name="wvm"></param>
        /// <returns></returns>
        private static List<Season> CreateSPSeasons(WaitViewModel wvm)
        {
            wvm.MaximumValue = 297;
            var result = new List<Season>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 1; i <= 22; i++)
            {
                var num = i;

                result.Add(new Season
                {
                    Number = num,
                    Description = $"Description of Season_{num}",
                    ImageBytes = imageBytes.ToArray(),
                    Episodes = new List<Episode>(CreateSPEpisodes(wvm, num, stopWatch))
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
        private static List<Episode> CreateSPEpisodes(WaitViewModel wvm, int seasonNum, Stopwatch stopwatch)
        {
            var result = new List<Episode>();
            var episodesCount = GetSPEpisodesCount(seasonNum);

            for (var i = 0; i < episodesCount; i++)
            {
                var num = i + 1;
                wvm.CurrentValue++;
                var addresses = GetSPEpAddressesSPOnline(seasonNum, num);
                var duration = GetEpisodeDuration(addresses.First());
                var fullNum = seasonNum * 100 + num;

                result.Add(new Episode
                {
                    Name = $"Episode_{num}",
                    Number = num,
                    SeasonNumber = seasonNum,
                    Description = $"Description of Episode_{num}",
                    Addresses = new List<string>(addresses),
                    SelectedAddress = addresses.First(),
                    PrevChainLink = GetPerviousChainEpisodeNumber(fullNum),
                    NextChainLink = GetNextChainEpisodeNumber(fullNum),
                    ImageBytes = imageBytes.ToArray(),
                    Checked = true,
                    TotalDuration = duration,
                    EpisodeEndTime = duration
                });

                wvm.RemainingTime = stopwatch.Elapsed;
            }

            return result;
        }

        /// <summary>
        /// Получить номер предыдущего эпизода в цепи эпизодов
        /// </summary>
        /// <param name="epFullNum">Полный номер эпизода</param>
        /// <returns></returns>
        private static int GetPerviousChainEpisodeNumber(int epFullNum)
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
        private static int GetNextChainEpisodeNumber(int epFullNum)
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

        /// <summary>
        /// Получение длительноси фильма
        /// </summary>
        /// <param name="address">Адрес фильма</param>
        /// <returns></returns>
        private static TimeSpan GetEpisodeDuration(string address)
        {
            VlcPlayer.Play(new Uri(address));
            while (!VlcPlayer.IsPlaying())
            {
                Thread.Sleep(50);
                Thread.Yield();
                if (VlcPlayer.CouldPlay) continue;

                WinMan.ShowWindow(new DialogViewModel(
                                      $"Адрес {address} возможно не существует, либо нет интернет соединения",
                                      DialogType.INFO));
                VlcPlayer.Stop();

                return new TimeSpan();
            }

            var result = TimeSpan.FromMilliseconds(VlcPlayer.Length);

            VlcPlayer.Stop();
            Thread.Yield();

            return result;
        }

        /// <summary>
        /// Создать список джамперов
        /// </summary>
        /// <param name="count">Количество создаваемых джамперов</param>
        /// <param name="episodeDuration">Длительность эпизода</param>
        /// <returns></returns>
        public static List<Jumper> CreateJumpers(int count, TimeSpan episodeDuration)
        {
            rnd = new Random();
            var result = new List<Jumper>();

            for (var i = 0; i < count; i++)
            {
                var num = i + 1;
                switch (num)
                {
                    case 1:
                        result.Add(new Jumper
                        {
                            StartTime = new TimeSpan(),
                            EndTime = new TimeSpan(0, 0, rnd.Next(7, 16))
                        });
                        break;
                    case 2:
                        var start = new TimeSpan(0,
                                                 rnd.Next(rnd.Next(3, 7)),
                                                 rnd.Next(60));
                        result.Add(new Jumper
                        {
                            StartTime = start,
                            EndTime = start + new TimeSpan(0, 0, rnd.Next(10, 40))
                        });
                        break;
                    case 3:
                        var end = episodeDuration - new TimeSpan(0, 0, rnd.Next(15, 30));
                        result.Add(new Jumper
                        {
                            StartTime = end - new TimeSpan(0, 0, rnd.Next(10, 40)),
                            EndTime = end
                        });
                        break;
                }
            }

            return result;
        }

        #region Методы сайта online-south-park.ru

        /// <summary>
        /// Получить список адресов эпизодов фильма Южный Парк
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <param name="episodeNum">Номер эпизода</param>
        /// <returns></returns>
        private static List<string> GetSPEpAddressesSPOnline(int seasonNum, int episodeNum)
        {
            var result = new List<string>();

            if (seasonNum == 15 &&
                episodeNum == 3)
            {
                return new List<string> { "https://serv1.freehat.cc/cdn_yripmaq/sppar/1503/1503_par.m3u8" };
            }

            var ip = GetSPSeasonIpSPOnline(seasonNum);
            var voicesCount = GetSPEpVoicesCountSPOnline(seasonNum, episodeNum);
            string voice;

            if (voicesCount > 1)
            {
                for (var i = 1; i <= voicesCount; i++)
                {
                    var num = i;
                    voice = GetSPEpVoiceNameSPOnline(seasonNum, num);
                    result.Add(
                        $"http://{ip}/video/spark/s{seasonNum:00}-{voice}/{GetSPEpAddressNumSPOnline(seasonNum, episodeNum, voice)}.mp4");
                }

                return result;
            }

            voice = GetSPEpVoiceNameSPOnline(seasonNum);

            return new List<string>
            {
                $"http://{ip}/video/spark/s{seasonNum:00}-{voice}/{GetSPEpAddressNumSPOnline(seasonNum, episodeNum, voice)}.mp4"
            };
        }

        /// <summary>
        /// Получить количество озвучек эпизода фильма Южный Парк
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <param name="episodeNum">Номер эпизода</param>
        /// <returns></returns>
        private static int GetSPEpVoicesCountSPOnline(int seasonNum, int episodeNum)
        {
            switch (seasonNum)
            {
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

                case 1:
                case 2:
                case 16:
                case 17:
                case 18:
                case 20:
                case 21:
                    return 2;
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
                            return "paramount";
                        default:
                            return "goblin";
                    }
                case 2:
                    switch (voiceNum)
                    {
                        case 1:
                            return "mtv";
                        default:
                            return "goblin";
                    }
                case 14:
                    switch (voiceNum)
                    {
                        case 1:
                            return "kubik";
                        default:
                            return "mtv";
                    }
                case 10:
                    return "paramount";
                case 19:
                    switch (voiceNum)
                    {
                        case 1:
                            return "paramount";
                        default:
                            return "kubik";
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
                            return "kubik";
                        default:
                            return "paramount";
                    }
                default:
                    return "mtv";
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
                case "goblin":
                    switch (seasonNum)
                    {
                        case 1:
                            return $"{seasonNum}{episodeNum:00}";
                        default:
                            return $"{episodeNum:00}";
                    }
                case "mtv":
                    switch (seasonNum)
                    {
                        case 9:
                            return $"{seasonNum}{episodeNum:00}";
                        case 14:
                            switch (episodeNum)
                            {
                                case 5:
                                case 6:
                                    return "";
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
                                    return "";
                                default:
                                    return $"{seasonNum}{episodeNum:00}";
                            }
                        case 22:
                            switch (episodeNum)
                            {
                                case 4:
                                case 9:
                                    return "";
                                default:
                                    return $"{episodeNum:00}";
                            }
                        default:
                            return $"{episodeNum:00}";
                    }
                case "kubik":
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

        #endregion
    }
}
