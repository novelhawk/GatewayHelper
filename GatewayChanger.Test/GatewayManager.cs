using System;

namespace GatewayChanger.Test
{
    public class GatewayManager
    {
        private Gateway[] _gateways;
        private int _currentGateway;

        public void LoadGatewaysFromFile(string path)
        {
            _gateways = new[]
            {
                new Gateway(192, 168, 1, 1),
                new Gateway(192, 168, 1, 254)
            };
        }

        /// <summary>
        /// Verifies that the current gateway is present in the array, otherwise changes it to the first entry
        /// </summary>
        public void EnsureCorrectGateway()
        {
            if (_gateways.Length == 0)
            {
                Console.WriteLine("[WARNING] There are no gateways in the collection.");
                return;
            }
            
            var gateways = GatewayChanger.GetGateways();

            if (gateways.Length == 0)
            {
                Console.WriteLine("[WARNING] There are no gateways in the forward table.");
                return;
            }
            
            if (gateways.Length > 1)
                Console.WriteLine("[WARNING] Multiple gateways identified, using first.");

            var current = gateways[0];
            var index = Array.IndexOf(_gateways, gateways[0]);
            _currentGateway = index;

            if (index < 0)
            {
                Console.WriteLine("[WARNING] Current gateway \"{0}\" is not present in the array, switching to \"{1}\".", current, _gateways[0]);
                GatewayChanger.ChangeGateway(_gateways[0]);
                _currentGateway = 0;
            }
        }
        
        public void UseNext()
        {
            if (_gateways.Length == 0) // Would throw DivideByZeroException
            {
                Console.WriteLine("[WARNING] Attempted to change gateway with no gateways in the array.");
                return;
            }
            
            _currentGateway++;
            _currentGateway %= _gateways.Length;

            GatewayChanger.ChangeGateway(_gateways[_currentGateway]);
        }

        public Gateway Gateway => _gateways[_currentGateway];
    }
}