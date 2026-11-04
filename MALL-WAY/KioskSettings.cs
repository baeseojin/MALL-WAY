using System;
using System.IO;
using Newtonsoft.Json;

namespace MallWay
{
    public sealed class KioskSettings
    {
        public string MallName { get; set; }
        public string DatabasePath { get; set; }
        public string StartNodeId { get; set; }
        public int DefaultFloor { get; set; }

        public static KioskSettings Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kiosk-settings.json");
            var settings = JsonConvert.DeserializeObject<KioskSettings>(File.ReadAllText(path));
            if (settings == null || string.IsNullOrWhiteSpace(settings.MallName)
                || string.IsNullOrWhiteSpace(settings.DatabasePath)
                || string.IsNullOrWhiteSpace(settings.StartNodeId) || settings.DefaultFloor < 1)
            {
                throw new InvalidDataException("kiosk-settings.json의 필수 설정을 확인해 주세요.");
            }

            settings.DatabasePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                settings.DatabasePath));
            return settings;
        }
    }
}
