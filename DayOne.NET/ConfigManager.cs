using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DayOne.NET
{
    public class ConfigManager
    {
        public static string DEFALUT_CONFIG_FILE = "dayone_config.xml";

        public static readonly string BaseEntryPath = @"\Apps\Day One\Journal.dayone\entries";

        public static readonly string BasePhotoPath = @"\Apps\Day One\Journal.dayone\photos"; 

        public static string DropBoxPath { get; set; }

        public static string EntryPath { get; set; }

        public static string PhotoPath { get; set; }

        public static void LoadConfig()
        {
            LoadConfig(DEFALUT_CONFIG_FILE);
        }

        public static void LoadConfig(string file)
        {
            if (System.IO.File.Exists(file)) {
                XmlDocument xml = new XmlDocument();
                xml.Load(file);

                var GetValue = new Func<string, string>((path) => {
                    var selectedNode = xml.SelectSingleNode(path);
                    return selectedNode.Attributes["Value"].Value;
                });

                DropBoxPath = GetValue("/Common/DropBox/Path");
                EntryPath = GetValue("/Common/DropBox/EntryPath");
                PhotoPath = GetValue("/Common/DropBox/PhotoPath");
            }
        }

        public static void SaveConfig()
        {
            SaveConfig(DEFALUT_CONFIG_FILE);
        }

        public static void SaveConfig(string file)
        {
            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8)) {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;

                writer.WriteStartDocument();
                writer.WriteStartElement("Common");
                writer.WriteStartElement("DropBox");

                writer.WriteStartElement("Path");
                writer.WriteAttributeString("Value", DropBoxPath);
                writer.WriteEndElement();

                writer.WriteStartElement("EntryPath");
                writer.WriteAttributeString("Value", EntryPath);
                writer.WriteEndElement();

                writer.WriteStartElement("PhotoPath");
                writer.WriteAttributeString("Value", PhotoPath);
                writer.WriteEndElement();

                writer.WriteEndElement();   // end of DropBox
                writer.WriteEndElement();   // end of Common
            }
        }
    }
}
