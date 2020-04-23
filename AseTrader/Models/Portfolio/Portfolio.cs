using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.Portfolio
{
    public class Portfolio
    {
        public Portfolio(IReadOnlyList<IPosition> portList)
        {
            Positions = portList;
        }

        public IReadOnlyList<IPosition> Positions { get; set; }

    }
}
