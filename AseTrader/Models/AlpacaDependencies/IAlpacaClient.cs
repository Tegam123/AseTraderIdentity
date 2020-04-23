﻿using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.AlpacaDependencies
{
   public interface IAlpacaClient
    {
        public AlpacaTradingClient AlpacaClientTrading();
        public AlpacaDataClient AlpacaClientData();
        public AlpacaStreamingClient AlpacaClientStreaming();

    }
}