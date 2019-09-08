using System;

namespace ConsolePolygon
{
    using System.IO;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            //SimpleDBTest();
            //AdvancedDBTest();
            //HardDBTest();
        }

        private static void SimpleDBTest()
        {
            var dbTest = new SimpleDBPolygon();

            for (var i = 0; i < 2; i++)
            {
                dbTest.CreatingSimpleDB();
            }
        }

        private static void AdvancedDBTest()
        {
            var dbTest = new AdvancedDBPolygon();

            for (var i = 0; i < 2; i++)
            {
                dbTest.CreatingAdvancedDB();
            }
        }

        private static void HardDBTest()
        {
            var dbTest = new HardDBPolygon();

            for(var i = 0; i< 2; i++)
            {
                dbTest.CreatingHardDB();
            }
        }
    }
}
