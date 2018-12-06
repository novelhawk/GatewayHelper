using System;
using System.Net;

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

        public Gateway(long ip)
        {
            _bytes = BitConverter.GetBytes((uint) ip);
        }
        
        public long Address => BitConverter.ToUInt32(_bytes, 0); 

        public static implicit operator IPAddress(Gateway gateway)
        {
            return new IPAddress(gateway._bytes);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", _bytes[0], _bytes[1], _bytes[2], _bytes[3]);
        }

        public bool Equals(Gateway other)
        {
            for (int i = 0; i < 4; i++)
                if (!Equals(_bytes[i], other._bytes[i]))
                    return false;
            return true;
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