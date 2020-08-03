namespace VzduchDotek.Net.AirTouch
{
    public class AirTouchResponse
    {
        public AirTouchResponse(string[] msg)
        {
            Content = msg;
        }

        public string[] Content { get; private set; }
    }
}