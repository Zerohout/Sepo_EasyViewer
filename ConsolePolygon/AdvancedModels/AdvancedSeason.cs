namespace ConsolePolygon.AdvancedModels
{
    using System.Collections.Generic;

    public class AdvancedSeason
    {
        public AdvancedSeason()
        {
            AdvancedEpisodes = new List<AdvancedEpisode>();
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<AdvancedEpisode> AdvancedEpisodes { get; set; }
    }
}
