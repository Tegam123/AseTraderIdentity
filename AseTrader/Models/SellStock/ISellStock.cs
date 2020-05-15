using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Models.Alpaca_dependency;

namespace AseTrader.Models.SellStock
{
    public interface ISellStock
    {
        Task SellStocks(string sym, int quantity, Decimal price, IAlpacaClient alpaca);
        void SellStocks_http(string sym, int quantity, Decimal price, string accesstoken);
    }
}
