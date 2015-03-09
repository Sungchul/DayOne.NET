using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayOne.NET
{
    public class ConfigManager
    {
        private static string configFileName = "dayone_config.xml";

        public static readonly string BaseEntryPath = @"\Apps\Day One\Journal.dayone\entries";

        public static readonly string BasePhotoPath = @"\Apps\Day One\Journal.dayone\photos"; 

        public static string DropBoxPath { get; set; }

        public static string EntryPath { get; set; }

        public static string PhotoPath { get; set; }




        public static void LoadConfig()
        {
        }

        public static void SaveConfig()
        {
        }
    }
}
