using System;
using System.Collections.Generic;
using System.Text;

namespace ConsolePolygon
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AdvancedModels;
    using LiteDB;

    public class AdvancedDBPolygon
    {
        public void CreatingAdvancedDB()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(
                currentAssembly?.Location ??
                AppDomain.CurrentDomain.BaseDirectory).DirectoryName;

            using (var db = new LiteDatabase($"{currentDirectory}/advancedTest.db"))
            {
                var count = 2;

                var serials = db.GetCollection<AdvancedSerial>("serials");
                var serialsCount = serials.Count();
                var seasons = db.GetCollection<AdvancedSeason>("seasons");
                var seasonsCount = seasons.Count();
                var episodes = db.GetCollection<AdvancedEpisode>("episodes");
                var episodesCount = episodes.Count();

                var episodeList = CreateAdvancedEpisodes(count * count * count);
                var seasonList = CreateAdvancedSeasons(count * count, episodeList, count);
                var serialList = CreateAdvancedSerials(count, seasonList, count);


                if (serialsCount < 1)
                {
                    serials.Insert(serialList);
                    Console.WriteLine("Коллекция сериалов добавлена");
                }
                else
                {
                    var list = serials
                               .IncludeAll()
                               .FindAll().ToArray();

                    for (var i = 0; i < serialsCount; i++)
                    {
                        var num = i;
                        var elem = list[num];

                        Console.WriteLine("Найден " + elem.Name);
                        Console.WriteLine($"В наличии {elem.AdvancedSeasons.Count} сезонов");
                        Console.WriteLine($"В каждом сезоне по {elem.AdvancedSeasons.First().AdvancedEpisodes.Count} эпизодов");
                        Console.WriteLine();
                    }
                }
                

                if (seasonsCount < 1)
                {
                    seasons.Insert(seasonList);
                    Console.WriteLine("Коллекция сезонов добавлена");
                }
                else
                {
                    var list = seasons
                               .IncludeAll()
                               .FindAll().ToArray();

                    for (var i = 0; i < seasonsCount; i++)
                    {
                        var num = i;
                        var elem = list[num];

                        Console.WriteLine("Найден " + elem.Name);
                        Console.WriteLine($"В наличии {elem.AdvancedEpisodes.Count} эпизодов");
                        Console.WriteLine();
                    }
                }

                if (episodesCount < 1)
                {
                    episodes.Insert(episodeList);
                    Console.WriteLine("Коллекция эпизодов добавлена");
                }
                else
                {
                    var list = episodes.FindAll().ToArray();

                    for (var i = 0; i < episodesCount; i++)
                    {
                        var num = i;
                        Console.WriteLine("Найден " + list[num].Name);
                    }
                }

            }
        }

        /// <summary>
        /// Создание списка сериалов
        /// </summary>
        /// <param name="count">Количество сериалов в списке</param>
        /// <param name="seasonList">Весь список сезонов</param>
        /// <param name="seasonsCount">Количество сезонов в сериале</param>
        /// <returns></returns>
        private List<AdvancedSerial> CreateAdvancedSerials(int count, List<AdvancedSeason> seasonList, int seasonsCount)
        {
            var result = new List<AdvancedSerial>();
            var list = new List<AdvancedSeason>(seasonList);

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new AdvancedSerial
                {
                    Name = $"Serial_{num}",
                    Description = $"Description of Serial_{num}",
                    SerialType = $"Type of Serial_{num}"
                });

                for (var j = 0; j < seasonsCount; j++)
                {
                    result.Last().AdvancedSeasons.Add(list.First());
                    list.RemoveAt(0);
                }
            }

            return result;
        }

        /// <summary>
        /// Создание списка сезонов
        /// </summary>
        /// <param name="count">Количество сезонов в списке</param>
        /// <param name="episodeList">Список всех эпизодов</param>
        /// <param name="episodesCount">Количество эпизодов в сезоне</param>
        /// <returns></returns>
        private List<AdvancedSeason> CreateAdvancedSeasons(int count, List<AdvancedEpisode> episodeList, int episodesCount)
        {
            var result = new List<AdvancedSeason>();
            var list = new List<AdvancedEpisode>(episodeList);

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new AdvancedSeason
                {
                    Name = $"Season_{num}",
                    Number = num,
                    Description = $"Description of Season_{num}"
                });

                for (var j = 0; j < episodesCount; j++)
                {
                    result.Last().AdvancedEpisodes.Add(list.First());
                    list.RemoveAt(0);
                }
            }

            return result;
        }

        /// <summary>
        /// Создание списка эпизодов
        /// </summary>
        /// <param name="count">Количество эпизодов в списке</param>
        /// <returns></returns>
        private List<AdvancedEpisode> CreateAdvancedEpisodes(int count)
        {
            var result = new List<AdvancedEpisode>();

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new AdvancedEpisode
                {
                    Name = $"Episode_{num}",
                    Number = num,
                    Description = $"Description of Episode_{num}"
                });
            }

            return result;
        }
    }
}
