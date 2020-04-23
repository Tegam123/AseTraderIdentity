using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.SellStock
{
    public interface ISellStock
    {
        void SellStocks_http(string sym, int quantity, Decimal price, string accesstoken);
    }
}
