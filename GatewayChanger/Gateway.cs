using System;

namespace GatewayChanger
{
    public struct Gateway : IEquatable<Gateway>
    {
        private readonly byte[] _bytes;

        public Gateway(byte first, byte second, byte third, byte fourth)
        {
            _bytes = new[]
            {
                first, second, third, fourth
            };
        }

        public Gateway(uint ip)
        {
            _bytes = BitConverter.GetBytes(ip);
        }
        
        public static implicit operator uint(Gateway gateway)
        {
            return BitConverter.ToUInt32(gateway._bytes, 0);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", _bytes[0], _bytes[1], _bytes[2], _bytes[3]);
        }

        public bool Equals(Gateway other)
        {
            return Equals(_bytes[0], other._bytes[0]) &&
                   Equals(_bytes[1], other._bytes[1]) &&
                   Equals(_bytes[2], other._bytes[2]) &&
                   Equals(_bytes[3], other._bytes[3]);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Gateway gateway && Equals(gateway);
        }

        public override int GetHashCode()
        {
            return _bytes != null ? _bytes.GetHashCode() : 0;
        }
    }
}