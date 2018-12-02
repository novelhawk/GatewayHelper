using System;

namespace ConnectionSwitcher
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
            return $"{_bytes[0]}.{_bytes[1]}.{_bytes[2]}.{_bytes[3]}";
        }

        public bool Equals(Gateway other)
        {
            return Equals(_bytes, other._bytes);
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