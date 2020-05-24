using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.Stock_info_builder
{
    public class SellStockBuilder
    {
        public string stock_symbol { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string email { get; set; }
    }
}
