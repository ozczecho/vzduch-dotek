namespace VzduchDotek.Net.AirTouch
{
    public class Sensor
    {
        public Sensor(int id)
        {
            Id = id;
        }
        public int Id {get; private set;}
        public bool IsAvailable {get;set;}
        public bool IsLowBattery {get;set;}
        public int Temperature {get;set;}
    }
}
