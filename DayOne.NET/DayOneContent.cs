using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayOne.NET
{
    public class DayOneContent
    {
        public string Activity { get; set; }

        public DateTime CreationDate { get; set; }

        #region Creator

        public string DeviceAgent { get; set; }

        public DateTime GenerationDate { get; set; }

        public string HostName { get; set; }

        public string OsAgent { get; set; }

        public string SoftwareAgent { get; set; }

        #endregion

        public string EntryText { get; set; }

        public bool IgnoreStepCount { get; set; }

        #region Location

        public string AdministrativeArea { get; set; }

        public string Country { get; set; }

        public double Latitude { get; set; }

        public string Locality { get; set; }

        public double Longitude { get; set; }

        public string PlaceName { get; set; }

        #region Region

        public double CenterLatitude { get; set; }

        public double CenterLongitude { get; set; }

        public double Radius { get; set; }

        #endregion
        
        #endregion

        public bool Starred { get; set; }

        public int StepCount { get; set; }

        public string TimeZone { get; set; }

        public string UUID { get; set; }

        #region Weather

        public string Celsius { get; set; }

        public string Description { get; set; }

        public string Fahrenheit { get; set; }

        public string IconName { get; set; }

        public int PressureMB { get; set; }

        public string Service { get; set; }

        public int RelativeHumidity { get; set; }

        public DateTime SunriseDate { get; set; }

        public DateTime SunsetDate { get; set; }
        
        public int VisibilityKM { get; set; }

        public int WindBearing { get; set; }

        public int WindChillCelsius { get; set; }

        public int WindSpeedKPH { get; set; }
        
        
        #endregion

        public string GetInformation()
        {
            return CreationDate.ToString();
        }

        public static DayOneContent Create()
        {
            return new DayOneContent() {
                CreationDate = DateTime.Now.ToUniversalTime(),
                
                DeviceAgent = "Microsoft Windows",
                GenerationDate = DateTime.Now.ToUniversalTime(),
                HostName = Environment.MachineName,
                OsAgent = Environment.OSVersion.ToString(),
                SoftwareAgent = "Day One .NET/0.0.1",

                IgnoreStepCount = true,

                AdministrativeArea = "Daejeon",
                Country = "South Korea",
                Latitude = 36.418725047072911,
                Locality = "Yuseonggu",
                Longitude = 127.39112867620702,
                PlaceName = "우리집",

                CenterLatitude = 36.418831098692777,
                CenterLongitude = 127.39205340000001,
                Radius = 70.867135407212473,
                
                Starred = false,
                StepCount = 0,
                TimeZone = "Asia / Seoul",
                UUID = Guid.NewGuid().ToString("N"),
            };
        }

        public static DayOneContent ReadContents(byte[] data)
        {
            var entry = Plist.readPlist(data) as Dictionary<string, object>;
            return ReadContents(entry);
        }

        public static DayOneContent ReadContents(string path)
        {
            var entry = Plist.readPlist(path) as Dictionary<string, object>;
            return ReadContents(entry);
        }

        private static DayOneContent ReadContents(Dictionary<string, object> entry)
        {
            var content = new DayOneContent();

            if (entry.Keys.Contains("Activity"))
                content.Activity = (string)entry["Activity"];

            if (entry.Keys.Contains("Creation Date"))
                content.CreationDate = (DateTime)entry["Creation Date"];

            if (entry.Keys.Contains("Creator")) {
                var creator = entry["Creator"] as Dictionary<string, object>;

                if (creator.Keys.Contains("Device Agent"))
                    content.DeviceAgent = (string)creator["Device Agent"];

                if (creator.Keys.Contains("Generation Date"))
                    content.GenerationDate = (DateTime)creator["Generation Date"];

                if (creator.Keys.Contains("Host Name"))
                    content.HostName = (string)creator["Host Name"];

                if (creator.Keys.Contains("OS Agent"))
                    content.OsAgent = (string)creator["OS Agent"];

                if (creator.Keys.Contains("Software Agent"))
                    content.SoftwareAgent = (string)creator["Software Agent"];
            }

            if (entry.Keys.Contains("Entry Text")) {
                content.EntryText = (string)entry["Entry Text"];
            }

            if (entry.Keys.Contains("Ignore Step Count")) {
                content.IgnoreStepCount = (bool)entry["Ignore Step Count"];
            }

            if (entry.Keys.Contains("Location")) {
                var location = entry["Location"] as Dictionary<string, object>;

                if (location.Keys.Contains("Administrative Area"))
                    content.AdministrativeArea = (string)location["Administrative Area"];

                if (location.Keys.Contains("Country"))
                    content.Country = (string)location["Country"];

                if (location.Keys.Contains("Latitude"))
                    content.Latitude = (double)location["Latitude"];

                if (location.Keys.Contains("Locality"))
                    content.Locality = (string)location["Locality"];

                if (location.Keys.Contains("Longitude"))
                    content.Longitude = (double)location["Longitude"];

                if (location.Keys.Contains("Place Name"))
                    content.PlaceName = (string)location["Place Name"];

                if (location.Keys.Contains("Region")) {
                    var region = location["Region"] as Dictionary<string, object>;

                    if (region.Keys.Contains("Center")) {
                        var center = region["Center"] as Dictionary<string, object>;

                        if (center.Keys.Contains("Latitude"))
                            content.CenterLatitude = (double)location["Latitude"];

                        if (center.Keys.Contains("Longitude"))
                            content.CenterLongitude = (double)location["Longitude"];
                    }

                    if (region.Keys.Contains("Radius"))
                        content.Radius = (double)region["Radius"];
                }
            }

            if (entry.Keys.Contains("Starred"))
                content.Starred = (bool)entry["Starred"];

            if (entry.Keys.Contains("Step Count"))
                content.StepCount = (int)entry["Step Count"];

            if (entry.Keys.Contains("Time Zone"))
                content.TimeZone = (string)entry["Time Zone"];

            if (entry.Keys.Contains("UUID"))
                content.UUID = (string)entry["UUID"];

            if (entry.Keys.Contains("Weather")) {
                var weather = entry["Weather"] as Dictionary<string, object>;

                if (weather.Keys.Contains("Celsius"))
                    content.Celsius = (string)weather["Celsius"];

                if (weather.Keys.Contains("Description"))
                    content.Description = (string)weather["Description"];

                if (weather.Keys.Contains("Fahrenheit"))
                    content.Fahrenheit = (string)weather["Fahrenheit"];

                if (weather.Keys.Contains("IconName"))
                    content.IconName = (string)weather["IconName"];

                if (weather.Keys.Contains("Pressure MB"))
                    content.PressureMB = (int)weather["Pressure MB"];

                if (weather.Keys.Contains("Relative Humidity"))
                    content.RelativeHumidity = (int)weather["Relative Humidity"];

                if (weather.Keys.Contains("Service"))
                    content.Service = (string)weather["Service"];

                if (weather.Keys.Contains("Sunrise Date"))
                    content.SunriseDate = (DateTime)weather["Sunrise Date"];

                if (weather.Keys.Contains("Sunset Date"))
                    content.SunsetDate = (DateTime)weather["Sunset Date"];

                if (weather.Keys.Contains("Visibility KM"))
                    content.VisibilityKM = Convert.ToInt32(weather["Visibility KM"]);

                if (weather.Keys.Contains("Wind Bearing"))
                    content.WindBearing = (int)weather["Wind Bearing"];

                if (weather.Keys.Contains("Wind Chill Celsius"))
                    content.WindChillCelsius = (int)weather["Wind Chill Celsius"];

                if (weather.Keys.Contains("Wind Speed KPH"))
                    content.WindSpeedKPH = (int)weather["Wind Speed KPH"];
            }

            return content;
        }

        public static void SaveDayOneContent(string path, DayOneContent content)
        {
            var entry = new Dictionary<string, object>();
            entry.Add("Activity", content.Activity);
            entry.Add("Creation Date", content.CreationDate);

            var creator = new Dictionary<string, object>();
            entry.Add("Creator", creator);
            creator.Add("Device Agent", content.DeviceAgent);
            creator.Add("Generation Date", content.GenerationDate);
            creator.Add("Host Name", content.HostName);
            creator.Add("OS Agent", content.OsAgent);
            creator.Add("Software Agent", content.SoftwareAgent);

            entry.Add("Entry Text", content.EntryText);
            entry.Add("Ignore Step Count", content.IgnoreStepCount);

            var location = new Dictionary<string, object>();
            entry.Add("Location", location);
            location.Add("Administrative Area", content.AdministrativeArea);
            location.Add("Country", content.Country);
            location.Add("Latitude", content.Latitude);
            location.Add("Locality", content.Locality);
            location.Add("Longitude", content.Longitude);
            location.Add("Place Name", content.PlaceName);

            var region = new Dictionary<string, object>();
            location.Add("Region", region);

            var center = new Dictionary<string, object>();
            region.Add("Center", center);
            center.Add("Latitude", content.CenterLatitude);
            center.Add("Longitude", content.CenterLongitude);
            region.Add("Radius", content.Radius);

            entry.Add("Starred", content.Starred);
            entry.Add("Step Count", content.StepCount);
            entry.Add("Time Zone", content.TimeZone);
            entry.Add("UUID", content.UUID);

            //var weather = new Dictionary<string, object>();
            //entry.Add("Weather", weather);
            //weather.Add("Celsius", content.Celsius);
            //weather.Add("Description", content.Description);
            //weather.Add("Fahrenheit", content.Fahrenheit);
            //weather.Add("IconName", content.IconName);
            //weather.Add("Pressure MB", content.PressureMB);
            //weather.Add("Relative Humidity", content.RelativeHumidity);
            //weather.Add("Service", content.Service);
            //weather.Add("Sunrise Date", content.SunriseDate);
            //weather.Add("Sunset Date", content.SunsetDate);
            //weather.Add("Visibility KM", content.VisibilityKM);
            //weather.Add("Wind Bearing", content.WindBearing);
            //weather.Add("Wind Chill Celsius", content.WindChillCelsius);
            //weather.Add("Wind Speed KPH", content.WindSpeedKPH);

            Plist.writeXml(entry, path);
        }
    }
}
