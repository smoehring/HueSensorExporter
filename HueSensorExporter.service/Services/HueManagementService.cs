using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueApi.Models;
using HueApi.Models.Sensors;

namespace HueSensorExporter.service.Services
{
    public class HueManagementService
    {
		private readonly Dictionary<Guid, RoomConfiguration> _roomMapping;
        private Dictionary<Guid, string> _sensorMapping;

        public IReadOnlyDictionary<Guid, RoomConfiguration> RoomMapping => _roomMapping;
        public IReadOnlyDictionary<Guid, string> SensorMapping => _sensorMapping;

        public HueManagementService()
        {
            _roomMapping = new Dictionary<Guid, RoomConfiguration>();
        }

        public void MapRooms(IReadOnlyCollection<Room> rooms)
        {
            foreach (var room in rooms)
            {
                foreach (var child in room.Children)
                {
                    _roomMapping.Add(child.Rid, new RoomConfiguration(room.Id, room.Metadata?.Name ?? room.Id.ToString("D")));
                }
            }
        }

        public void MapSensors(List<Device> sensorsData)
        {
            _sensorMapping = sensorsData.ToDictionary(resource => resource.Id,
                resource => resource.Metadata?.Name ?? resource.Id.ToString("D"));
        }
    }

    public record RoomConfiguration
    {
        public RoomConfiguration(Guid guid, string displayName)
        {
            Guid = guid;
            DisplayName = displayName;
        }

        public Guid Guid { get; set; }
        public string DisplayName { get; set; }
    }
}
