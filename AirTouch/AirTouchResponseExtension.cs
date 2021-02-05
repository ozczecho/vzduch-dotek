using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;

namespace VzduchDotek.Net.AirTouch
{
    public static class AirTouchResponseExtension
    {
        public static int GetAcId(this AirTouchResponse response)
        {
            var id = Convert.ToInt32(response.Content[MessageConstants.AirconId], 2);
            Log.Verbose($"Aircon Id is {id}. Raw {response.Content[MessageConstants.AirconId]}");

            return id;
        }

        public static int GetAcBrandId(this AirTouchResponse response)
        {
            var id = Convert.ToInt32(response.Content[MessageConstants.AirconBrandId]);
            Log.Verbose($"Aircon BrandId is {id}. Raw {response.Content[MessageConstants.AirconBrandId]}");

            return id;
        }
        public static AcStatus GetAirconPowerStatus(this AirTouchResponse response)
        {
            var value = response.Content[MessageConstants.AirconStatus];
            var status = value.JavaStyleSubstring(0, 1) == "1" ? AcStatus.AcOn : AcStatus.AcOff;
            Log.Verbose($"Aircon is {status}. Raw {response.Content[MessageConstants.AirconStatus]}");

            return status;
        }
        public static string GetAirconStatus(this AirTouchResponse response)
        {
            var value = response.Content[MessageConstants.AirconStatus];
            var status = value.JavaStyleSubstring(1, 2) == "1" ? "ERROR" : "OK";
            Log.Verbose($"Aircon is {status}. Raw {response.Content[MessageConstants.AirconStatus]}");

            return status;
        }
        public static string GetSystemName(this AirTouchResponse response)
        {
            char[] systemName = new char[16];
            for (int i = 0; i < 16; i++)
            {
                systemName[i] = Convert.ToChar(Convert.ToInt32(response.Content[MessageConstants.SystemNameStart + i], 2));
            }
            int nameCharLength = 16;
            int x = 15;
            while (x >= 0 && systemName[x] == ' ')
            {
                nameCharLength--;
                x--;
            }
            var name = new string(systemName.Where(x=> ! char.IsControl(x)).ToArray());
            Log.Verbose($"Aircon system name is {name}");

            return name;
        }

        public static List<Sensor> GetSensors(this AirTouchResponse response)
        {
            var sensors = new List<Sensor>{};
            for (var i = 0; i < 32; i++)
            {
                var sensor = new Sensor(i);
                var isAvailable = Convert.ToInt32(response.Content[MessageConstants.SensorDataStart + i].JavaStyleSubstring(0, 1), 2);
                var isLowBattery = Convert.ToInt32(response.Content[MessageConstants.SensorDataStart + i].JavaStyleSubstring(1, 2), 2);
                sensor.Temperature = Convert.ToInt32(response.Content[MessageConstants.SensorDataStart + i].JavaStyleSubstring(2, 8), 2);
                sensor.IsAvailable = isAvailable == 1;
                sensor.IsLowBattery = isLowBattery == 1;
                Log.Verbose($"Sensor {i} IsAvailable {sensor.IsAvailable} LowBattery {sensor.IsLowBattery} Temperature {sensor.Temperature}");
                
                sensors.Add(sensor);
            }
            return sensors;
        }
        public static AcMode GetAcMode(this AirTouchResponse response)
        {
            var acMode = Convert.ToInt32(response.Content[MessageConstants.AirconMode].JavaStyleSubstring(1, 8), 2);
            var acModeEnum = AcMode.Auto;
            switch (acMode)
            {
                case 0:
                    acModeEnum = AcMode.Auto;
                    break;
                case 1:
                    acModeEnum = AcMode.Heat;
                    break;
                case 2:
                    acModeEnum = AcMode.Dry;
                    break;
                case 3:
                    acModeEnum = AcMode.Fan;
                    break;
                case 4:
                    acModeEnum = AcMode.Cool;
                    break;
            }
            Log.Verbose($" Ac Mode is {acMode} - enum {acModeEnum}");

            return acModeEnum;
        }
        public static AcFanMode GetFanSpeed(this AirTouchResponse response)
        {
            var fanspeed = Convert.ToInt32(response.Content[MessageConstants.FanSpeed].JavaStyleSubstring(4, 8), 2);
            var fanspeedEnum = AcFanMode.Auto;
            switch (fanspeed)
            {
                case 0:
                    fanspeedEnum = AcFanMode.Auto;
                    break;
                case 1:
                    fanspeedEnum = AcFanMode.Low;
                    break;
                case 2:
                    fanspeedEnum = AcFanMode.Medium;
                    break;
                case 3:
                    fanspeedEnum = AcFanMode.High;
                    break;
                case 4:
                    fanspeedEnum = AcFanMode.Auto;
                    break;
            }
            Log.Verbose($" Fanspeed is {fanspeed} - enum {fanspeedEnum}");

            return fanspeedEnum;
        }
        public static string GetAirTouchId(this AirTouchResponse response)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 8; i++)
            {
                builder.Append(Convert.ToInt32(response.Content[MessageConstants.AirTouchIdStart + i].JavaStyleSubstring(4, 8), 2));
            }
            var airTouchId = builder.ToString();
            Log.Verbose($"AirTouchId is  {airTouchId} ");

            return airTouchId;
        }
        public static int GetTouchPadGroupId(this AirTouchResponse response)
        {
            var id = Convert.ToInt32(response.Content[MessageConstants.TouchpadGroupId], 2);
            Log.Verbose($"Touchpad GroupId is  {id} ");

            return id;
        }
        public static int GetTouchPadTemperature(this AirTouchResponse response)
        {
            var temperature = Convert.ToInt32(response.Content[MessageConstants.TouchpadTemperature].JavaStyleSubstring(1, 8), 2);
            Log.Verbose($"Touchpad Temperature is  {temperature} ");

            return temperature;
        }
        public static int GetDesiredTemperature(this AirTouchResponse response)
        {
            var temperature = Convert.ToInt32(response.Content[MessageConstants.DesiredTemperature].JavaStyleSubstring(1, 8), 2);
            Log.Verbose($"Desired Temperature is  {temperature} ");

            return temperature;
        }
        public static int GetRoomTemperature(this AirTouchResponse response)
        {
            var temperature = Convert.ToInt32(response.Content[MessageConstants.RoomTemperature], 2);
            Log.Verbose($"Room Temperature is  {temperature} ");

            return temperature;
        }
        public static int GetThermostatMode(this AirTouchResponse response)
        {
            var thermostatMode = Convert.ToInt32(response.Content[MessageConstants.ThermostatMode], 2);
            Log.Verbose($"Thermostat mode is  {thermostatMode} ");

            return thermostatMode;
        }
        public static List<Zone> GetZones(this AirTouchResponse response, int touchPadGroupId, int touchPadTemperature, List<Sensor> sensors)
        {
            string[] zoneData = new string[16];
            for (var i = 0; i < 16; i++)
            {
                zoneData[i] = response.Content[MessageConstants.ZoneDataStart + i];
                Log.Verbose($"Zone Data {zoneData[i]} for {i}");
                // Convert.ToString("10000000".JavaStyleSubstring(0,1)) -- Means zone is ON.
            }

            string[] groupData = new string[16];
            for (var i = 0; i < 16; i++)
            {
                groupData[i] = response.Content[MessageConstants.GroupDataStart + i];
            }

            string[] groupPercentageData = new string[16];
            for (var i = 0; i <= 15; i++)
            {
                groupPercentageData[i] = response.Content[MessageConstants.GroupPercentageDataStart + i];
            }

            string[] groupSetting= new string[16];
            for (var i = 0; i <= 15; i++)
            {
                groupSetting[i] = response.Content[MessageConstants.GroupSettingStart + i];
            }

            var groupName = new String[128]; 
            for (var i = 0; i < 128; i++)
            {
                groupName[i] = response.Content[MessageConstants.GroupNameStart + i];
            }

            char[] zoneNames = new char[128];
            for (int i = 0; i <= 127; i++)
            {
                zoneNames[i] = Convert.ToChar(Convert.ToInt32(groupName[i], 2));
            }


            var zones = new List<Zone> { };
            var numberOfZones = Convert.ToInt32(Convert.ToString(response.Content[MessageConstants.NumberOfZones]), 2);
            for (int i = 0; i < numberOfZones; i++)
            {
                var zone = new Zone(touchPadTemperature);
                StringBuilder sb = new StringBuilder { };
                for (int x = i * 8; x < (i + 1) * 8; x++)
                {
                    var character = (char)Convert.ToInt32(zoneNames[x]);
                    if (! char.IsControl(character))
                        sb.Append(Convert.ToString(character));
                }
                Log.Verbose($"Zone Name {sb.ToString()} for {i}");
                zone.Id = i;
                zone.Name = sb.ToString();
                
                int startZone = Convert.ToInt32(groupData[i].JavaStyleSubstring(0, 4), 2);
                zone.Status = zoneData[startZone].JavaStyleSubstring(0, 1) == "1" ? ZoneStatus.ZoneOn : ZoneStatus.ZoneOff;
                zone.FanValue = zone.Status == ZoneStatus.ZoneOn ? Convert.ToInt32(groupPercentageData[i].JavaStyleSubstring(1, 8), 2) * 5 : 0; //Only valid when zone is turned on...set to zero otherwise
                zone.DesiredTemperature = Convert.ToInt32(groupSetting[i].JavaStyleSubstring(3, 8), 2) + 1;
                zone.IsSpill = zoneData[i].JavaStyleSubstring(1, 2) == "1";
                Log.Verbose($"Zone Data {zoneData[startZone]} for Start Zone {startZone} and status is {zone.Status}");

                int feedback = Convert.ToInt32(groupSetting[i].JavaStyleSubstring(0, 3), 2);
                if ((touchPadGroupId - 1) != i)
                {
                    switch (feedback)
                    {
                        case 0:
                            zone.ZoneTemperatureType =ZoneTemperatureType.Hide;
                            break;
                        case 1:
                            var sensor1 = sensors.FirstOrDefault(x=> x.Id == (i * 2) && x.IsAvailable);
                            if (sensor1 == null)
                                sensor1 = sensors.FirstOrDefault(x => x.Id == ((i * 2) + 1) && x.IsAvailable);
                            if (sensor1 != null)
                                zone.Sensors.Add(sensor1);
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseSensor;
                            break;
                        case 2:
                            var sensor2 = sensors.FirstOrDefault(x => x.Id == ((i * 2) + 1) && x.IsAvailable);
                            if (sensor2 == null)
                                sensor2 = sensors.FirstOrDefault(x => x.Id == (i * 2) && x.IsAvailable);
                            if (sensor2 != null)
                                zone.Sensors.Add(sensor2);
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseSensor;
                            break;
                        case 3:
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseAverage;
                            zone.Sensors.AddRange(sensors.Where(x => (x.Id == ((i * 2) + 1) || x.Id == (i * 2)) && x.IsAvailable));
                            break;
                    }
                }
                else
                {
                    switch (feedback)
                    {
                        case 0:
                            zone.ZoneTemperatureType = ZoneTemperatureType.Hide;
                            break;
                        case 1:
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseTouchPad;
                            break;
                        case 2:
                            var sensor1 = sensors.FirstOrDefault(x => x.Id == (i * 2) && x.IsAvailable);
                            if (sensor1 == null)
                                sensor1 = sensors.FirstOrDefault(x => x.Id == ((i * 2) + 1) && x.IsAvailable);
                            if (sensor1 != null)
                                zone.Sensors.Add(sensor1);
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseSensor;
                            break;
                        case 3:
                            var sensor2 = sensors.FirstOrDefault(x => x.Id == ((i * 2) + 1) && x.IsAvailable);
                            if (sensor2 == null)
                                sensor2 = sensors.FirstOrDefault(x => x.Id == (i * 2) && x.IsAvailable);
                            if (sensor2 != null)
                                zone.Sensors.Add(sensor2);
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseSensor;
                            break;
                        case 4:
                            zone.ZoneTemperatureType = ZoneTemperatureType.UseAverage;
                            zone.Sensors.AddRange(sensors.Where(x => (x.Id == ((i * 2) + 1) || x.Id == (i * 2)) && x.IsAvailable));
                            break;
                    }
                }
                zones.Add(zone);
            }

            return zones;
        }
    }
}
