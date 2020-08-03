using System.Collections.Generic;

namespace VzduchDotek.Net.AirTouch
{
    public class MessageResponseParser
    {
        public AirTouchSystem Parse(AirTouchResponse response)
        {
            var ac = new Aircon();
            ac.Id = response.GetAcId();
            ac.BrandId = response.GetAcBrandId();
            ac.PowerStatus = response.GetAirconPowerStatus();
            ac.Status = response.GetAirconStatus();
            ac.Name = response.GetSystemName();
            ac.Sensors = response.GetSensors();
            ac.Mode = response.GetAcMode();
            ac.FanMode = response.GetFanSpeed();
            ac.AirTouchId = response.GetAirTouchId();
            ac.TouchPadGroupId = response.GetTouchPadGroupId();
            ac.TouchPadTemperature = response.GetTouchPadTemperature();
            ac.DesiredTemperature = response.GetDesiredTemperature();
            ac.RoomTemperature = response.GetRoomTemperature();
            ac.Zones = response.GetZones(ac.TouchPadGroupId, ac.TouchPadTemperature, ac.Sensors);

            return new AirTouchSystem(new List<Aircon> {ac});
        }
    }
}