using System;
using VzduchDotek.Net.AirTouch;

namespace VzduchDotek.Net.TcpMessaging
{
    public interface ITcpClient : IDisposable
    {
        AirTouchResponse ConnectAndSend(byte[] msg);
    }
}
