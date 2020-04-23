using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AseTrader.Models.AlpacaDependencies
{
    internal sealed class AlpacaClient : IAlpacaClient
    {

        private string API_KEY = "PKU7XMXXF1YKJ3F90Y0E";

        private string API_SECRET = "K07EfXfEyEmSAs8Axo75yKEVe1TiqFWwwA0gdzIZ";

        //private AlpacaTradingClient _alpacaTradingClient;
        private AlpacaDataClient _alpacaDataClient;
        private AlpacaStreamingClient _alpacaStreamingClient;

        public AlpacaDataClient AlpacaClientData()
        {
            // _alpacaDataClient = Environments.Paper.GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));
            return _alpacaDataClient;
        }

        public AlpacaStreamingClient AlpacaClientStreaming()
        {
            // _alpacaStreamingClient = Environments.Paper.GetAlpacaStreamingClient(new SecretKey(API_KEY, API_SECRET));
            return _alpacaStreamingClient;
        }

        public AlpacaTradingClient AlpacaClientTrading()
        {
            //Test af nye api funktioner
            //var _alpacaTradingClient = Environments.Paper.GetAlpacaTradingClient(API_KEY, new SecretKey(API_SECRET));

            var _alpacaTradingClient = Environments.Paper.GetAlpacaTradingClient(new SecretKey(API_KEY,API_SECRET));
            return _alpacaTradingClient;
        }
    }
}
