using Alpaca.Markets;

namespace AseTrader.Models.Alpaca_dependency
{
    public interface IAlpacaClient
    {
        public AlpacaTradingClient AlpacaClientTrading();
        public AlpacaDataClient AlpacaClientData();
        public AlpacaStreamingClient AlpacaClientStreaming();
    }
}