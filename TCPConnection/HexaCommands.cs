using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnection
{
    public class HexaCommands
    {
        public static byte[] GetBytes(int number)
        {
            return new byte[0];
        }

        public static ushort CalcCRC16(byte[] data)
        {
            ushort wCRC = 0;
            for (int i = 0; i < data.Length; i++)
            {
                wCRC ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCRC & 0x8000) != 0)
                        wCRC = (ushort)((wCRC << 1) ^ 0x1021);
                    else
                        wCRC <<= 1;
                }
            }
            return wCRC;
        }
    }
}
