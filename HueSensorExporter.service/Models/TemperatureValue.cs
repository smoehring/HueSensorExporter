using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HueSensorExporter.service.Models
{
    public class TemperatureValue
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("temperature_valid")]
        public bool Valid { get; set; }

        public TemperatureReport Report { get; set; }
    }

    public class TemperatureReport
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("changed")]
        public DateTime Changed { get; set; }
    }
}
