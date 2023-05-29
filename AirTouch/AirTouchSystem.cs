using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VzduchDotek.Net.AirTouch
{
    public class AirTouchSystem
    {
        public AirTouchSystem(List<Aircon> aircons)
        {
            Aircons = aircons;
            if (Aircons.Count == 1)
                SelectedAc = 0;
            else
            {
                if (Aircons == null)
                    throw new System.Exception("Need one aircon. None Supplied");
                else if (Aircons.Count > 1)
                    throw new System.Exception("Dual AC not supported");
            }
        }

        public List<Aircon> Aircons {get; private set;}

        public int SelectedAc {get; private set; }

        public Aircon   GetSelectedAircon()
        {
            return Aircons[SelectedAc];
        }
    }
    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(AirTouchSystem))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
