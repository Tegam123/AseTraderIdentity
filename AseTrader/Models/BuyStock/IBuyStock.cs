using AseTrader.Models.Alpaca_dependency;
using System;
using System.Threading.Tasks;

namespace AseTrader.Models.BuyStock
{
    public interface IBuyStock
    {
        Task BuyStocks(string sym, int quantity, Decimal price, IAlpacaClient alpaca);

        void BuyStocks_http(string sym, int quantity, Decimal price, string accesstoken);
    }
}