using HueApi.Models;
using HueApi.Models.Sensors;
using HueSensorExporter.service.Services;

namespace HueSensorExporter.service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly LocalHueApiService _hueApi;
        private readonly HueMetricService _metricService;
        private readonly HueManagementService _managementService;
        private readonly PeriodicTimer _timer;

        public Worker(ILogger<Worker> logger, LocalHueApiService hueApi, HueMetricService metricService, HueManagementService managementService)
        {
            _logger = logger;
            _hueApi = hueApi;
            _metricService = metricService;
            _managementService = managementService;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rooms = await _hueApi.GetRoomsAsync();
            _managementService.MapRooms(rooms.Data);
            var sensors = await _hueApi.GetDevicesAsync();
            _managementService.MapSensors(sensors.Data);


            await FetchData();
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await FetchData();
            }
        }

        private async Task FetchData()
        {
            await FetchTemperature();
            await FetchLightlevel();
            await FetchLightState();
        }

        private async Task FetchLightState()
        {
            var lights = await _hueApi.GetLightsAsync();
            _metricService.SetLightState(lights.Data);
        }

        private async Task FetchLightlevel()
        {
            var lightLevel = await _hueApi.GetLightLevelsAsync();
            _metricService.SetLightLevel(lightLevel.Data);
        }

        private async Task FetchTemperature()
        {
            var temperatures = await _hueApi.GetTemperaturesAsync();
            _metricService.SetTemperature(temperatures.Data);
        }
    }
}