﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace UAM.PTO
{
    public abstract class PNM
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public byte[] Raster { get; private set; }
        public int Stride { get { return Width * 6; } }

        public static PNM LoadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.GetEncoding(28591)))
            {
                string header = ReadToken(reader);
                switch (header)
                {
                    case "P1":
                        return new PlainPBM(reader);
                    case "P2":
                        return new PlainPGM(reader);
                    case "P3":
                        return new PlainPPM(reader);
                    case "P4":
                        return new RawPBM(reader);
                    case "P5":
                        return new RawPGM(reader);
                    case "P6":
                        return new RawPPM(reader);
                    default:
                        throw new MalformedFileException("Malformed header");
                }
            }
        }

        public static void SaveFile(string path)
        {

        }

        // read token, ignore comments, throw MalformedFileException if there is no token to read
        // comment hash in ascii is 35
        protected static string ReadToken(TextReader reader)
        {
            StringBuilder builder = new StringBuilder();
            // Skip starting whitespace
            int temp;
            while(true)
            {
                temp = reader.Peek();
                if(temp == -1)
                    throw new MalformedFileException();
                if (temp == 35)
                {
                    reader.ReadLine();
                    continue;
                }
                if(!char.IsWhiteSpace((char)temp))
                    break;
                //comment - read to the end of line
                reader.Read();
            }
            // Read actual token
            builder.Append((char)reader.Read());
            while (true)
            {
                temp = reader.Peek();
                if (temp == -1 || char.IsWhiteSpace((char)temp))
                    break;
                builder.Append((char)reader.Read());
            }
            // Return
            return builder.ToString();
        }

        protected void InitializeRaster()
        {
            Raster = new byte[Width * Height * 6];
        }

        protected int ParseNumber(string token)
        {
            int result;
            if (!Int32.TryParse(token, System.Globalization.NumberStyles.None, NumberFormatInfo.InvariantInfo, out result))
                throw new MalformedFileException();
            return result;
        }

        protected int ParseNumber(string token, int min, int maxval)
        {
            int result;
            if (!Int32.TryParse(token, System.Globalization.NumberStyles.None, NumberFormatInfo.InvariantInfo, out result)
                || result > maxval || result < min)
                throw new MalformedFileException();
            return result;
        }

        // 0,0 is upper left corner, indices are postitive
        protected void ColorPixel(int index, ushort r, ushort g, ushort b)
        {
            if (index >= (Width * Height))
                throw new ArgumentException();
            int realIndex = index * 6;
            Buffer.SetByte(Raster, realIndex, (byte)r );
            Buffer.SetByte(Raster, ++realIndex, (byte)(r >> 8));
            Buffer.SetByte(Raster, ++realIndex, (byte)g);
            Buffer.SetByte(Raster, ++realIndex, (byte)(g >> 8));
            Buffer.SetByte(Raster, ++realIndex, (byte)b);
            Buffer.SetByte(Raster, ++realIndex, (byte)(b >> 8));
        }

        // 0,0 is upper left corner, indices are postitive
        protected void ColorPixel(int x, int y, ushort r, ushort g, ushort b)
        {
            ColorPixel((x * Width) + y, r, g, b);
        }
    }
}
