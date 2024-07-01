using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using VzduchDotek.Net.AirTouch;
using VzduchDotek.Net.TcpMessaging;

namespace VzduchDotek.Net.Controllers
{
    [ApiController]
    [Route("api")]
    public class VzduchDotekController : ControllerBase
    {
        private readonly ITcpClient _client;
        private readonly AirTouchMessages _atMessages;
 
        public VzduchDotekController(ITcpClient client, AirTouchMessages atMessages)
        {
            _client = client;
            _atMessages = atMessages;
        }

        [HttpGet("aircons")]
        public object Get()
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var response = JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/switch/{status}")]
        public object Switch(int selectedId, AcStatus status)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            if (status != at.GetSelectedAircon().PowerStatus)
            {
              result = _client.ConnectAndSend(_atMessages.ToggleAcOnOff(at.SelectedAc));
              at = parser.Parse(result);
            }

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/toggle")]
        public object Toggle()
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            result = _client.ConnectAndSend(_atMessages.ToggleAcOnOff(at.SelectedAc));
            at = parser.Parse(result);

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/zones/{zoneId}/switch/{status}")]
        public object ZoneSwitch(int selectedId, int zoneId, ZoneStatus status)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var zone = at.GetSelectedAircon().GetZoneById(zoneId);
            if (zone != null && zone.Status != status)
            {
                result = _client.ConnectAndSend(_atMessages.ToggleZone(zoneId));
                at = parser.Parse(result);
            }
            else
            {
                if (zone == null)
                  throw new Exception("Failed to find selected zone");
            }

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/zones/{zoneId}/toggle")]
        public object ZoneToggle(int selectedId, int zoneId)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var zone = at.GetSelectedAircon().GetZoneById(zoneId);
            if (zone != null)
            {
                result = _client.ConnectAndSend(_atMessages.ToggleZone(zoneId));
                at = parser.Parse(result);
            }
            else
              throw new Exception("Failed to find selected zone");

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/modes/{mode}")] 
        public object SetAcMode(int selectedId, AcMode mode)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var ac = at.GetSelectedAircon();
            if (ac != null)
            {
                result = _client.ConnectAndSend(_atMessages.SetMode(at.SelectedAc, ac.BrandId, (int)mode));
                at = parser.Parse(result);
            }
            else
                throw new Exception("Failed to find selected aircon unit");

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/fanmodes/{mode}")]
        public object SetAcFanMode(int selectedId, AcFanMode mode)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var ac = at.GetSelectedAircon();
            if (ac != null)
            {
                result = _client.ConnectAndSend(_atMessages.SetFanSpeed(at.SelectedAc, ac.BrandId, (int)mode));
                at = parser.Parse(result);
            }
            else
                throw new Exception("Failed to find selected aircon unit");

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/temperature/{incDec}")]
        public object SetTemperature(int selectedId, AcTemperature incDec)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var ac = at.GetSelectedAircon();
            if (ac != null)
            {
                result = _client.ConnectAndSend(_atMessages.SetNewTemperature(at.SelectedAc, (int)incDec));
                at = parser.Parse(result);
            }
            else
                throw new Exception("Failed to find selected aircon unit");

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/zones/{zoneId}/temperature/{incDec}")]
        ///
        /// If zone is temperature managed, this api will set the desired zone temp
        /// If zone is fan managed, this api will set fan percentage
        ///
        public object SetZoneFan(int selectedId, int zoneId, AcTemperature incDec)
        {
            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);

            var ac = at.GetSelectedAircon();
            if (ac != null)
            {
                result = _client.ConnectAndSend(_atMessages.SetFan(zoneId, (int)incDec));
                at = parser.Parse(result);
            }
            else
                throw new Exception("Failed to find selected aircon unit");

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }

        [HttpPost("aircons/{selectedId}/zones/{zoneId}/damper/{percentage}")]
        public object SetDamperPercentage(int selectedId, int zoneId, int percentage)
        {
            Log.ForContext<VzduchDotekController>().Debug("AC [{Ac}] Zone [{Zone}] Percentage [{Percentage}]", selectedId, zoneId, percentage);

            if (percentage < 0 || percentage > 100)
                throw new Exception("Requested percentage must be between 0 and 100%");

            var result = _client.ConnectAndSend(_atMessages.GetInitMsg());
            var parser = new MessageResponseParser();
            var at = parser.Parse(result);
            var ac = at.GetSelectedAircon();
            if (ac == null)
                throw new Exception("Failed to find selected aircon unit");


            if (ac.Zones[zoneId].Status == ZoneStatus.ZoneOff)
            {
                Log.ForContext<VzduchDotekController>().Debug("Turning Zone [{ZoneId}] ON", zoneId);
                ZoneToggle(selectedId, zoneId);
                result = _client.ConnectAndSend(_atMessages.GetInitMsg());
                at = parser.Parse(result);
                ac = at.GetSelectedAircon();
            }

            var requestedPercentage = 5 * (int)Math.Round(percentage / 5.0);
            var currentPercentage = ac.Zones[zoneId].FanValue;
            var incDec = currentPercentage > requestedPercentage ? AcTemperature.Decrement : AcTemperature.Increment;
            var runCount = 0;
            while (currentPercentage != requestedPercentage && runCount < 21)
            {
                var cancellationToken = new CancellationTokenSource(9000);
                try
                {
                    var t = Task.Run(async delegate
                    {
                        _client.ConnectAndSend(_atMessages.SetFan(zoneId, (int)incDec));
                        await Task.Delay(500);
                    });
                    t.Wait(cancellationToken.Token);

                    result = _client.ConnectAndSend(_atMessages.GetInitMsg());
                    at = parser.Parse(result);
                    ac = at.GetSelectedAircon();
                    currentPercentage = ac.Zones[zoneId].FanValue;

                    Log.ForContext<VzduchDotekController>().Debug("Zone [{ZoneId}] Current Percentage [{Current}] Requested [{Requested}]", zoneId, currentPercentage, requestedPercentage);
                }
                catch (OperationCanceledException ex)
                {
                    Log.ForContext<VzduchDotekController>().Warning("OperationCanceledException: Zone [{ZoneId}] Current Percentage [{Current}] Requested [{Requested}] [{@ex}]", 
                    zoneId, currentPercentage, requestedPercentage, ex);
                }
                finally
                {
                    runCount++;
                }
            }

            var response = System.Text.Json.JsonSerializer.Serialize(at, typeof(AirTouchSystem), SourceGenerationContext.Default);
            Log.ForContext<VzduchDotekController>().Debug("{@AirTouchSystem}", at);

            return Content(response, "application/json");
        }
    }
}
