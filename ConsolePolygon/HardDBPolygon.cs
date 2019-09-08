namespace ConsolePolygon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using HardModels;
    using LiteDB;
    using static System.Console;

    public class HardDBPolygon
    {
        private List<HardSerial> _serials;
        private List<HardSeason> _seasons;
        private List<HardEpisode> _episodes;


        public void CreatingHardDB()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(
                currentAssembly?.Location ??
                AppDomain.CurrentDomain.BaseDirectory).DirectoryName;

            using (var db = new LiteDatabase($"{currentDirectory}/hardTest.db"))
            {
                var count = 2;

                var serials = db.GetCollection<HardSerial>();
                var serialsCount = serials.Count();
                var seasons = db.GetCollection<HardSeason>();
                var seasonsCount = seasons.Count();
                var episodes = db.GetCollection<HardEpisode>();
                var episodesCount = episodes.Count();


                if (episodesCount < 1 ||
                    seasonsCount < 1 ||
                    serialsCount < 1)
                {
                    episodes.Insert(CreateHardEpisodes(count * count * count));
                    WriteLine("Коллекция эпизодов добавлена");

                    seasons.Insert(
                        CreateHardSeasons(
                            count * count, episodes.FindAll().ToList(), count));
                    WriteLine("Коллекция сезонов добавлена");

                    serials.Insert(
                        CreateHardSerials(count, seasons
                                                 .FindAll()
                                                 .ToList(), count));
                    WriteLine("Коллекция сериалов добавлена\n");
                    return;
                }
                else
                {
                    _serials = new List<HardSerial>(serials.FindAll());
                    _seasons = new List<HardSeason>(seasons.FindAll());
                    _episodes = new List<HardEpisode>(episodes.FindAll());
                }
            }

            for (var i = 0; i < _serials.Count; i++)
            {
                var num = i;
                var elem = _serials[num];

                WriteLine("Найден " + elem.Name);
                WriteLine($"В наличии {elem.HardSeasons.Count} сезонов и {elem.HardEpisodes.Count} эпизодов");
                WriteLine($"В каждом сезоне по {elem.HardSeasons.First().HardEpisodes.Count} эпизодов\n");
            }

            for (var i = 0; i < _seasons.Count; i++)
            {
                var num = i;
                var elem = _seasons[num];

                var parent = _serials.First(s => s.HardSeasons
                                                  .Any(hs => hs.Id == elem.Id));

                WriteLine("Найден " + elem.Name);
                WriteLine($"Сезон относится к {parent.Name}");
                WriteLine($"В наличии {elem.HardEpisodes.Count} эпизодов");
                WriteLine();
            }

            for (var i = 0; i < _episodes.Count; i++)
            {
                var num = i;
                var elem = _episodes[num];
                var serialParent = _serials
                                   .First(s => s.HardEpisodes
                                                  .Any(he => he.Id == elem.Id));
                var seasonParent = _seasons
                                   .First(s => s.HardEpisodes
                                                  .Any(he => he.Id == elem.Id));

                WriteLine("Найден " + elem.Name);
                WriteLine($"Эпизод относится к {serialParent.Name} и к {seasonParent.Name}");
            }
        }

        /// <summary>
        /// Создание списка сериалов
        /// </summary>
        /// <param name="count">Количество создаваемых сериалов</param>
        /// <param name="seasonList">Общий список сезонов</param>
        /// <param name="seasonsCount">Количество сезонов в каждом сериале</param>
        /// <returns></returns>
        private List<HardSerial> CreateHardSerials(int count, List<HardSeason> seasonList, int seasonsCount)
        {
            var result = new List<HardSerial>();
            var list = new List<HardSeason>(seasonList);

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new HardSerial
                {
                    Name = $"Serial_{num}",
                    Description = $"Description of Serial_{num}",
                    SerialType = $"Type of Serial_{num}"
                });

                for (var j = 0; j < seasonsCount; j++)
                {
                    var season = list.First();
                    result[num].HardSeasons.Add(season);
                    result[num].HardEpisodes.AddRange(season.HardEpisodes);
                    list.RemoveAt(0);
                }
            }

            return result;
        }

        /// <summary>
        /// Создание списка сезонов
        /// </summary>
        /// <param name="count">Количество создаваемых сезонов</param>
        /// <param name="episodeList">Общий список эпизодов</param>
        /// <param name="episodesCount">Количество эпизодов в каждом сезоне</param>
        /// <returns></returns>
        private List<HardSeason> CreateHardSeasons(int count, List<HardEpisode> episodeList, int episodesCount)
        {
            var result = new List<HardSeason>();
            var list = new List<HardEpisode>(episodeList);

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new HardSeason
                {
                    Name = $"Season_{num}",
                    Number = num,
                    Description = $"Description of Season_{num}"
                });

                for (var j = 0; j < episodesCount; j++)
                {
                    result[num].HardEpisodes.Add(list.First());
                    list.RemoveAt(0);
                }
            }

            return result;
        }

        /// <summary>
        /// Содание списка эпизодов
        /// </summary>
        /// <param name="count">Количество создаваемых эпизодов</param>
        /// <returns></returns>
        private List<HardEpisode> CreateHardEpisodes(int count)
        {
            var result = new List<HardEpisode>();

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new HardEpisode
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
