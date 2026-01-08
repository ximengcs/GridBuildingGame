using System;
using System.Buffers.Binary;
using System.Text;

namespace SgFramework.Net
{
    public struct BinaryReader
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public bool Eof => Position >= Length;

        public bool ReadClean => Position == Length;
        private byte[] Body { get; }

        public BinaryReader(byte[] body)
        {
            Body = body;
            Position = 0;
            Length = body.Length;
        }

        public bool ReadBoolean()
        {
            var b = BitConverter.ToBoolean(Body, Position);
            Position += 1;
            return b;
        }

        public int ReadInt32()
        {
            var i = BinaryPrimitives.ReadInt32BigEndian(Body.AsSpan(Position));
            Position += 4;
            return i;
        }

        public short ReadInt16()
        {
            var i = BinaryPrimitives.ReadInt16BigEndian(Body.AsSpan(Position));
            Position += 2;
            return i;
        }

        public string ReadString(short len)
        {
            var s = Encoding.UTF8.GetString(Body, Position, len);
            Position += len;
            return s;
        }

        public Memory<byte> ReadBytes(int len)
        {
            var ba = Body.AsMemory(Position, len);
            Position += len;
            return ba;
        }
    }
}