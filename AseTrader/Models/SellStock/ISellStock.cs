using AseTrader.Models.Alpaca_dependency;
using System;
using System.Threading.Tasks;

namespace AseTrader.Models.SellStock
{
    public interface ISellStock
    {
        Task SellStocks(string sym, int quantity, Decimal price, IAlpacaClient alpaca);

        void SellStocks_http(string sym, int quantity, Decimal price, string accesstoken);
    }
}