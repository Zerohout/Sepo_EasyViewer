namespace ConsolePolygon.AdvancedModels
{
    using System.Collections.Generic;

    public class AdvancedSerial
    {
        public AdvancedSerial()
        {
            AdvancedSeasons = new List<AdvancedSeason>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialType { get; set; }

        public List<AdvancedSeason> AdvancedSeasons { get; set; }
    }
}
