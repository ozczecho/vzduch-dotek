using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace VzduchDotek.Net.AirTouch
{
    public class Zone
    {
        public Zone(int touchPadTemperature)
        {
            TouchPadTemperature = touchPadTemperature;
            Sensors = new List<Sensor>{};
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ZoneStatus Status { get; set; }
        public int FanValue {get;set;}
        public bool IsSpill {get;set;}
        public int DesiredTemperature {get;set;}
        public ZoneTemperatureType ZoneTemperatureType {get;set;}
        public List<Sensor> Sensors {get;set;}

        private int TouchPadTemperature {get;set;}
        public int GetTemperature()
        {
            switch (this.ZoneTemperatureType)
            {
                case ZoneTemperatureType.UseSensor:
                    var sensor = Sensors.FirstOrDefault();
                    if (sensor != null)
                        return sensor.Temperature;
                    else
                    {
                        Log.Warning($"Getting Zone Temperature. ZoneTemperatureType {this.ZoneTemperatureType} expecting one sensor, got none");
                        return 0;
                    }
                case ZoneTemperatureType.UseTouchPad:
                    return TouchPadTemperature;
                case ZoneTemperatureType.UseAverage:
                    var temps = Sensors.Sum(x=>x.Temperature) + TouchPadTemperature;
                    var sensorCount = Sensors.Count();
            
                    return (int)(temps) / (sensorCount + 1);
            }

            Log.Warning($"Getting Zone Temperature. ZoneTemperatureType {this.ZoneTemperatureType} not defined");
            return 0;
        }
    }
}
