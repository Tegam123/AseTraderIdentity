using System;
using Alpaca.Markets;

namespace AseTrader.Models.Alpaca_dependency
{
    internal sealed class AlpacaClient : IAlpacaClient
    {
        private string API_KEY = "PKU7XMXXF1YKJ3F90Y0E";

        private string API_SECRET = "K07EfXfEyEmSAs8Axo75yKEVe1TiqFWwwA0gdzIZ";

        private AlpacaTradingClient _alpacaTradingClient;
        private AlpacaDataClient _alpacaDataClient;
        private AlpacaStreamingClient _alpacaStreamingClient;

        public AlpacaTradingClient AlpacaClientTrading()
        {
            //_alpacaTradingClient = Environments.Paper.GetAlpacaTradingClient(new SecretKey(API_KEY,API_SECRET));
            return _alpacaTradingClient;
        }

        public AlpacaDataClient AlpacaClientData()
        {
            //_alpacaDataClient = Environments.Paper.GetAlpacaDataClient(new SecretKey(API_KEY,API_SECRET));
            return _alpacaDataClient;
        }

        public AlpacaStreamingClient AlpacaClientStreaming()
        {
            //_alpacaStreamingClient = Environments.Paper.GetAlpacaStreamingClient(new SecretKey(API_KEY, API_SECRET));
            return _alpacaStreamingClient;
        }
    }
}