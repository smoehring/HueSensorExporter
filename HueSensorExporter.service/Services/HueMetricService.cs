using System.Diagnostics.Metrics;
using System.Text.Json;
using HueApi.Models.Sensors;
using HueSensorExporter.service.Models;

namespace HueSensorExporter.service.Services
{
    public class HueMetricService
    {
        public static string MeterName => "smoehring.HueSensorExporter.HueSensor";
        private readonly HueManagementService _managementService;
        private readonly Meter _meter;
        private readonly Gauge<double> _temperatureGauge;

        public HueMetricService(HueManagementService managementService)
        {
            _managementService = managementService;
            _meter = new Meter(MeterName);

            _temperatureGauge = new Gauge<double>(_meter, "hue_sensor_temperature", "celsius",
                "Temperatures recorded by the Philips Hue Sensors");
        }


        public void SetTemperature(IReadOnlyList<TemperatureResource> resources)
        {
            foreach (var resource in resources)
            {
                var temperatureValue = resource.ExtensionData["temperature"].Deserialize<TemperatureValue>();
                var sensorname = _managementService.SensorMapping[resource.Owner.Rid];
                _temperatureGauge.Set(temperatureValue.Temperature, new KeyValuePair<string, object?>("sensor", sensorname));

            }
        }
    }
}
