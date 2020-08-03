using System;
using Serilog;

namespace VzduchDotek.Net.AirTouch
{
    public class AirTouchMessages
    {
        private byte[] _message;
        private byte[] _sumByte;

        public AirTouchMessages()
        {
            _message = new byte[13];
            _sumByte = new byte[_message.Length];
        }

        public byte[] GetInitMsg()
        {
            ResetContents();
            _message[1] = 1;
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] SetMode(int acId, int acBrandId, int mode)
        {
            ResetContents();
            int currentMode = mode;
            //if ((ExchData.GetSelectedAC() == 0 && ExchData.GetAC1().GetBrand() == 11) || (ExchData.GetSelectedAC() == 1 && ExchData.GetAC2().GetBrand() == 11))
            if (acId == 0 && acBrandId == 11)
            {
                switch (currentMode)
                {
                    case 0:
                        mode = 0;
                        break;
                    case 1:
                        mode = 2;
                        break;
                    case 2:
                        mode = 3;
                        break;
                    case 3:
                        mode = 4;
                        break;
                    case 4:
                        mode = 1;
                        break;
                }
            }
            //if ((ExchData.GetSelectedAC() == 0 && ExchData.GetAC1().GetBrand() == 15) || (ExchData.GetSelectedAC() == 1 && ExchData.GetAC2().GetBrand() == 15))
            if (acId == 0 && acBrandId == 15)
            {
                switch (currentMode)
                {
                    case 0:
                        mode = 5;
                        break;
                    case 1:
                        mode = 2;
                        break;
                    case 2:
                        mode = 3;
                        break;
                    case 3:
                        mode = 4;
                        break;
                    case 4:
                        mode = 1;
                        break;
                }
            }
            _message[1] = unchecked((byte)-122);
            _message[3] = (byte)acId;
            _message[4] = unchecked((byte)-127);
            _message[5] = (byte)mode;
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] SetFanSpeed(int acId, int acBrandId, int mode)
        {
            ResetContents();
            int currentMode = mode;
            //if ((ExchData.GetSelectedAC() == 0 && ExchData.GetAC1().GetBrand() == 15) || (ExchData.GetSelectedAC() == 1 && ExchData.GetAC2().GetBrand() == 15))
            if (acId == 0 && acBrandId == 15)
            {
                switch (currentMode)
                {
                    case 0:
                        mode = 4;
                        break;
                }
            }
            //if ((ExchData.GetSelectedAC() == 0 && ExchData.GetAC1().GetBrand() == 2) || (ExchData.GetSelectedAC() == 1 && ExchData.GetAC2().GetBrand() == 2))
            if (acId == 0 && acBrandId == 2)
            {
                switch (currentMode)
                {
                    case 0:
                        mode = 0;
                        break;
                    case 4:
                        mode = 1;
                        break;
                    default:
                        mode++;
                        break;
                }
            }
            _message[1] = unchecked((byte)-122);
            _message[3] = (byte)acId;
            _message[4] = unchecked((byte)-126);
            _message[5] = (byte)mode;
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] SetNewTemperature(int acId, int incDec)
        {
            ResetContents();
            _message[1] = unchecked((byte)-122);
            _message[3] = (byte)acId;
            if (incDec >= 0)
            {
                _message[4] = unchecked((byte)-93);
            }
            else if (incDec < 0)
            {
                _message[4] = unchecked((byte)-109);
            }
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] ToggleAcOnOff(int acId)
        {
            ResetContents();
            _message[1] = unchecked((byte)-122);
            _message[3] = (byte)acId;
            _message[4] = unchecked((byte)-128);
            _message[12] = CheckSum();

            return _message;
        }
        public byte[] ToggleZone(int room)
        {
            ResetContents();
            _message[1] = unchecked((byte)-127);
            _message[3] = (byte)room;
            _message[4] = unchecked((byte)-128);
            _message[12] = CheckSum();

            return _message;
        }
        public byte[] SetChangeTemperatureFan(int room)
        {
            ResetContents();
            _message[1] = unchecked((byte)-127);
            _message[3] = (byte)room;
            _message[4] = unchecked((byte)-128);
            _message[5] = 1;
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] SetFan(int room, string direction)
        {
            ResetContents();
            _message[1] = 1;
            _message[1] = unchecked((byte)-127);
            _message[3] = (byte)room;
            if (direction == "UP")
            {
                _message[4] = 2;
            }
            else
            {
                _message[4] = 1;
            }
            _message[5] = 1;
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] SetName(int id, string newName)
        {
            ResetContents();
            _message[1] = unchecked((byte)-123);
            _message[3] = (byte)((id - 1) + 128);
            byte[] newNameAsASC = StringToASCMax8(newName);
            for (int i = 0; i <= 7; i++)
            {
                _message[i + 4] = newNameAsASC[i];
            }
            _message[12] = CheckSum();

            return _message;
        }

        public byte[] SetACTimeMessage(int id, bool isTimeMsg, int hour, int minute)
        {
            ResetContents();
            _message[1] = unchecked((byte)-124);
            _message[3] = (byte)(id - 1);
            Log.Verbose("listener??" + _message[3]);
            if (isTimeMsg)
            {
                _message[4] = 0;
                Log.Verbose("hour: " + hour);
                Log.Verbose("minute: " + minute);
                _message[5] = (byte)hour;
                _message[6] = (byte)minute;
            }
            else
            {
                _message[4] = unchecked((byte)-127);
            }
            _message[12] = CheckSum();

            return _message;
        }
        private byte CheckSum()
        {
            int reSum;
            for (int i = 0; i <= _sumByte.Length - 1; i++)
            {
                _sumByte[i] = _message[i];
            }
            byte reSum2 = 0;
            int i2 = 0;
            while (i2 <= _sumByte.Length - 2)
            {
                byte b = _sumByte[i2];
                if (b >= 0)
                {
                    reSum = reSum2 + b;
                }
                else if (b == Byte.MinValue)
                {
                    reSum = reSum2 + 128;
                }
                else
                {
                    reSum = reSum2 + ((byte)(b + 256));
                }
                i2++;
                reSum2 = (byte)reSum;
            }
            return (byte)reSum2;
        }

        private void ResetContents()
        {
            Array.Fill(_message, (byte)0);
            _message[0] = 85;
            _message[2] = 12;
        }

        public byte[] StringToASCMax8(string s)
        {
            byte[] b = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                b[i] = 32;
            }
            char[] chars = s.ToCharArray();
            if (s.Length < 9)
            {
                for (int i2 = 0; i2 < s.Length; i2++)
                {
                    b[i2] = (byte)chars[i2];
                }
            }
            else
            {
                for (int i3 = 0; i3 < 8; i3++)
                {
                    b[i3] = (byte)chars[i3];
                }
            }
            return b;
        }

    }
}
