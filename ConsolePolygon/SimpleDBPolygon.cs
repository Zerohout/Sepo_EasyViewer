using System;
using System.Collections.Generic;
using System.Text;

namespace ConsolePolygon
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using LiteDB;
    using SimpleModels;

    public class SimpleDBPolygon
    {
        public void CreatingSimpleDB()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(
                currentAssembly?.Location ?? 
                AppDomain.CurrentDomain.BaseDirectory).DirectoryName;

            using (var db = new LiteDatabase($"{currentDirectory}/simpleTest.db"))
            {
                var serials = db.GetCollection<SimpleSerial>("serials");
                var serialsCount = serials.Count();
                var seasons = db.GetCollection<SimpleSeason>("seasons");
                var seasonsCount = seasons.Count();
                var episodes = db.GetCollection<SimpleEpisode>("episodes");
                var episodesCount = episodes.Count();

                if(serialsCount < 1)
                {
                    serials.Insert(CreateSimpleSerials(10));
                    Console.WriteLine("Коллекция сериалов добавлена");
                }
                else
                {
                    var list = serials.FindAll().ToArray();

                    for (var i = 0; i < serialsCount; i++)
                    {
                        var num = i;
                        Console.WriteLine("Найден " + list[num].Name);
                    }
                }

                

                if (seasonsCount < 1)
                {
                    seasons.Insert(CreateSimpleSeasons(10));
                    Console.WriteLine("Коллекция сезонов добавлена");
                }
                else
                {
                    var list = seasons.FindAll().ToArray();

                    for (var i = 0; i < seasonsCount; i++)
                    {
                        var num = i;
                        Console.WriteLine("Найден " + list[num].Name);
                    }
                }

                if (episodesCount < 1)
                {
                    episodes.Insert(CreateSimpleEpisodes(10));
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
        /// <returns></returns>
        private List<SimpleSerial> CreateSimpleSerials(int count)
        {
            var result = new List<SimpleSerial>();

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new SimpleSerial
                {
                    Name = $"Serial_{num}",
                    Description = $"Description of Serial_{num}",
                    SerialType = $"Type of Serial_{num}"
                });
            }

            return result;
        }

        /// <summary>
        /// Создание списка сезонов
        /// </summary>
        /// <param name="count">Количество сезонов в списке</param>
        /// <returns></returns>
        private List<SimpleSeason> CreateSimpleSeasons(int count)
        {
            var result = new List<SimpleSeason>();

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new SimpleSeason
                {
                    Name = $"Season_{num}",
                    Number = num,
                    Description = $"Description of Season_{num}"
                });
            }

            return result;
        }

        /// <summary>
        /// Создание списка эпизодов
        /// </summary>
        /// <param name="count">Количество эпизодов в списке</param>
        /// <returns></returns>
        private List<SimpleEpisode> CreateSimpleEpisodes(int count)
        {
            var result = new List<SimpleEpisode>();

            for (var i = 0; i < count; i++)
            {
                var num = i;

                result.Add(new SimpleEpisode
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
