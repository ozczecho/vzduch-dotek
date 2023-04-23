using System.Collections.Generic;
using System.Linq;

namespace VzduchDotek.Net.AirTouch
{
    public class Aircon
    {
        public Aircon()
        {
            Zones = new List<Zone> { };
            Sensors = new List<Sensor> {};
        }
        public int Id { get; set; }
        public string AirTouchId {get;set;}
        public AcMode Mode { get; set; }
        public string Name { get; set; }
        public AcStatus PowerStatus {get;set;}
        public string Status { get; set; }
        public int BrandId { get; set; }
        public int TouchPadGroupId {get;set;}
        public int TouchPadTemperature {get;set;}
        public int DesiredTemperature{get;set;}
        public int RoomTemperature {get;set;}
        public int ThermostatMode {get;set;}
        public AcFanMode FanMode { get; set; }

        public List<Zone> Zones { get; set; }

        public List<Sensor> Sensors {get;set;}
        public Zone GetZoneById(int id)
        {
            return Zones.FirstOrDefault(x=>x.Id == id);
        }

        public string Version => "0.7";
    }
}
