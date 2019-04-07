using System;
using System.Net;

namespace GatewayChanger
{
    public struct Gateway : IEquatable<Gateway>
    {
        private readonly byte _first;
        private readonly byte _second;
        private readonly byte _third;
        private readonly byte _fourth;

        public Gateway(byte first, byte second, byte third, byte fourth)
        {    
            _first  = first;
            _second = second;
            _third  = third;
            _fourth = fourth;
        }

        public Gateway(long ip)
        {
            _first  = (byte) (ip >> 0);
            _second = (byte) (ip >> 8);
            _third  = (byte) (ip >> 16);
            _fourth = (byte) (ip >> 24);
        }
        
        public byte[] Bytes => new[] {_first, _second, _third, _fourth};

        public long Address
        {
            get
            {
                uint address = _first;
                address |= (uint) (_second << 8);
                address |= (uint) (_third << 16);
                address |= (uint) (_fourth << 24);
                return address;
            }
        }

        public static implicit operator IPAddress(Gateway gateway)
        {
            return new IPAddress(gateway.Address);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", _first, _second, _third, _fourth);
        }

        public bool Equals(Gateway other)
        {
            return _first == other._first && 
                   _second == other._second && 
                   _third == other._third && 
                   _fourth == other._fourth;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Gateway other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _first.GetHashCode();
                hashCode = (hashCode * 397) ^ _second.GetHashCode();
                hashCode = (hashCode * 397) ^ _third.GetHashCode();
                hashCode = (hashCode * 397) ^ _fourth.GetHashCode();
                return hashCode;
            }
        }
    }
}