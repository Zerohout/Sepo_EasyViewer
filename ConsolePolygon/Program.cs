using System;

namespace ConsolePolygon
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using LiteDB;
    using Models.SerialModels;
    using Models.SettingModels;
    using Newtonsoft.Json;
    using static Helpers.Variables;

    class Program
    {
        private static Random rnd;

        public static string AppPath = new FileInfo(
            Assembly.GetEntryAssembly()?.Location ??
            AppDomain.CurrentDomain.BaseDirectory).DirectoryName;

        public const string AppDataFolderName = "AppData";
        public static readonly string AppDataPath = $"{AppPath}\\{AppDataFolderName}";




        static List<Episode> CheckedEpisodes = new List<Episode>();



        static void Main(string[] args)
        {
            rnd = new Random();

            // WriteLine($"{DateTime.Today:d}_{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}.txt");
            //CreatorTest.CreateSouthPark();
            //ReadKey();

            

            var ws1 = LoadOrCreateWatchingSettings();
            var ws2 = LoadOrCreateWatchingSettings();
            ws1.DefaultEpCount = 5;

            

            Console.WriteLine($"1-й тест - {IsEquals(ws1,ws2)}");
            Console.WriteLine($"2-й тест - {JsonConvert.SerializeObject(ws1)}");
            Console.WriteLine($"3-й тест - {JsonConvert.SerializeObject(ws2)}");


        }

        public static bool IsEquals(object obj1, object obj2) =>
            JsonConvert.SerializeObject(obj1) == JsonConvert.SerializeObject(obj2);

        public static WatchingSettings LoadOrCreateWatchingSettings()
        {
            WatchingSettings ws = new WatchingSettings();
            using (var db = new LiteDatabase(DBPath))
            {
                var genSet = db.GetCollection<WatchingSettings>();

                if (genSet.Count() < 1)
                {
                    genSet.Insert(new WatchingSettings());
                }

                ws = genSet.FindAll().Last();
            }

            return ws;
        }

        private static void WriteResult(List<Episode> episodes)
        {
            foreach (var ep in episodes)
            {
                Console.Write($"{episodes.IndexOf(ep)} - {ep.FullNumber} ");

                if (ep.PreviousChainEpisodeFullNumber == 0 &&
                    ep.NextChainEpisodeFullNumber == 0)
                {
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine($"предыдущий - {ep.PreviousChainEpisodeFullNumber}, следующий = {ep.NextChainEpisodeFullNumber}");
            }
        }

        




        

        private static List<Serial> GetSerials()
        {
            List<Serial> result;
            using (var db = new LiteDatabase(DBPath))
            {
                var serials = db.GetCollection<Serial>();

                result = serials.FindAll().ToList();
            }

            return result;
        }

        private static void UpdateDbEpisode(Episode ep)
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var serials = db.GetCollection<Serial>();

                var ser = serials.FindAll().ToList();

                ser.SelectMany(s => s.Episodes)
                   .First(e => e.SelectedAddress == ep.SelectedAddress)
                   .LastDateViewed = ep.LastDateViewed;

                serials.Update(ser);

            }
        }




    }
}
