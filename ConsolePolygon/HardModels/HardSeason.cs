namespace ConsolePolygon.HardModels
{
    using System.Collections.Generic;

    public class HardSeason
    {
        public HardSeason()
        {
            HardEpisodes = new List<HardEpisode>();
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //public HardSerial HardSerial { get; set; }
        public List<HardEpisode> HardEpisodes { get; set; }
    }
}
