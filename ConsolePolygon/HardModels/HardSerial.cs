namespace ConsolePolygon.HardModels
{
    using System.Collections.Generic;

    public class HardSerial
    {
        public HardSerial()
        {
            HardSeasons = new List<HardSeason>();
            HardEpisodes = new List<HardEpisode>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialType { get; set; }

        public List<HardSeason> HardSeasons { get; set; }
        public List<HardEpisode> HardEpisodes { get; set; }
    }
}
