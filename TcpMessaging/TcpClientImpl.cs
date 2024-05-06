using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using Serilog;
using VzduchDotek.Net.AirTouch;

namespace VzduchDotek.Net.TcpMessaging
{
    public class TcpClientImpl : ITcpClient
    {
        private TcpClient _client;
        private readonly AirTouchOptions _options;
        private bool disposedValue;

        public TcpClientImpl(IOptions<AirTouchOptions> options)
        {
            _options = options.Value;
            _client = new TcpClient();
        }

        public AirTouchResponse ConnectAndSend(byte[] msg)
        {
            if (! _client.Connected)
            {
                _client.Connect(_options.LocalHost, _options.LocalPort);
                Log.ForContext<TcpClientImpl>().Debug("Connected to {host} {port}",_options.LocalHost, _options.LocalPort);
            }
            else
                Log.ForContext<TcpClientImpl>().Debug("Using existing connection");

            var stream = _client.GetStream();
            stream.Write(msg, 0, msg.Length);

            int bytes = 0;
            Byte[] response = new Byte[1024];
            bytes = stream.Read(response, 0, response.Length);
            
            return FormatReceivedMessage(response);
        }

        private AirTouchResponse FormatReceivedMessage(byte[] receivedMessage)
        {
            var msgString = string.Empty;

            StringBuilder sbs = new StringBuilder();
            for (var i = 0; i < receivedMessage.Length; i++)
            {
                sbs.Append(receivedMessage[i].ToString("X2") + " ");
            }
            Log.ForContext<TcpClientImpl>().Verbose("Received Message {@Msg}", sbs.ToString());

            if (sbs.Length > 0)
            {
                string unused = msgString = msgString + sbs.ToString().Substring(startIndex: 0, sbs.Length - 1);
                Log.ForContext<TcpClientImpl>().Verbose("Service RecvMessage {Msg}", msgString);
            }

            string[] response;
            string[] rawTMP = msgString.Split(" ");
             
            if (rawTMP.Length < 395)
            {
                return null;
            }
            if (rawTMP.Length == 790)
            {
                response = CopyOfRange(rawTMP, 395, 790);
            }
            else
            {
                response = rawTMP;
            }

            for (var i = 0; i <= response.Length - 2; i++)
            {
                response[i] = ToFullBinaryString((int)long.Parse(response[i], NumberStyles.HexNumber)).Substring(startIndex: 24);
            }
            return new AirTouchResponse(response);
        }

        private T[] CopyOfRange<T>(T[] src, int start, int end)
        {
            var len = end - start;
            T[] dest = new T[len];
            Array.Copy(src, start, dest, 0, len);

            return dest;
        }

        private string ToFullBinaryString(int num)
        {
            char[] chs = new char[32];
            for (int i = 0; i < 32; i++)
            {
                chs[31 - i] = (char)(((num >> i) & 1) + 48);
            }
            return new string(chs);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_client != null)
                    {
                        if (_client.Connected)
                        {
                            var stream = _client.GetStream();
                            if (stream != null && stream.Socket != null && stream.Socket.Connected)
                            {
                                stream.Socket.Close();
                                stream.Close();
                            }
                        }
                        _client.Close();
                        _client = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}