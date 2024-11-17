using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace update
{
    public class Crc64Ecma
    {
        private static readonly ulong[] Table = new ulong[256];
        private const ulong Poly = 0xC96C5795D7870F42;

        static Crc64Ecma()
        {
            for (ulong i = 0; i < 256; i++)
            {
                ulong part = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((part & 1) != 0)
                    {
                        part = (part >> 1) ^ Poly;
                    }
                    else
                    {
                        part >>= 1;
                    }
                }
                Table[i] = part;
            }
        }

        public static ulong Compute(byte[] bytes)
        {
            ulong crc = 0xFFFFFFFFFFFFFFFF;
            foreach (byte b in bytes)
            {
                byte index = (byte)(((crc) & 0xFF) ^ b);
                crc = Table[index] ^ (crc >> 8);
            }
            return ~crc;
        }
        public static ulong ComputeFromBytes(byte[] bytes)
        {
            ulong crc = 0xFFFFFFFFFFFFFFFF;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte index = (byte)(((crc) & 0xFF) ^ bytes[i]);
                crc = Table[index] ^ (crc >> 8);
            }
            return ~crc;
        }
        public static ulong ComputeFromFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[1024 * 1024]; // 读取文件的缓冲区大小
                int bytesRead;
                ulong crc = 0xFFFFFFFFFFFFFFFF;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        byte index = (byte)(((crc) & 0xFF) ^ buffer[i]);
                        crc = Table[index] ^ (crc >> 8);
                    }
                }
                return ~crc;
            }
        }
    }
}
