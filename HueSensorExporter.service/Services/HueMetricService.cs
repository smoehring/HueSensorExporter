using System.Diagnostics.Metrics;
using System.Text.Json;
using HueApi.Models;
using HueApi.Models.Sensors;
using HueSensorExporter.service.Models;
using Light = HueApi.Models.Light;

namespace HueSensorExporter.service.Services
{
    public class HueMetricService
    {
        public static string MeterName => "smoehring.HueSensorExporter.HueSensor";
        private readonly HueManagementService _managementService;
        private readonly Meter _meter;
        private readonly Gauge<double> _temperatureGauge;
        private readonly Gauge<double> _lightlevelGauge;
        private readonly Gauge<double> _lampDimmingGauge;

        public HueMetricService(HueManagementService managementService)
        {
            _managementService = managementService;
            _meter = new Meter(MeterName);

            _temperatureGauge = new Gauge<double>(_meter, "hue_sensor_temperature", "celsius",
                "Temperatures recorded by the Philips Hue Sensors");

            _lightlevelGauge = new Gauge<double>(_meter, "hue_sensor_lightlevel", "lux",
                "Lightlevel recorded by the Philips Hue Sensors");

            _lampDimmingGauge = new Gauge<double>(_meter, "hue_lamp_brightness", "percentage",
                "Brightnes of a Lamp");
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

        public void SetLightLevel(List<LightLevel> lightLevelData)
        {
            foreach (var level in lightLevelData)
            {
                var sensorname = _managementService.SensorMapping[level.Owner.Rid];
                _lightlevelGauge.Set(level.Light.LuxLevel, new KeyValuePair<string, object?>("sensor", sensorname));
            }
        }

        public void SetLightState(List<Light> lightsData)
        {
            foreach (var light in lightsData)
            {
                var roomName = _managementService.RoomMapping[light.Owner.Rid].DisplayName;
                if (light.Dimming is null) continue;
                _lampDimmingGauge.Set(light.On.IsOn ? light.Dimming.Brightness : 0, new KeyValuePair<string, object?>("room", roomName), new KeyValuePair<string, object?>("lamp", light.Metadata.Name));
            }
        }
    }
}
