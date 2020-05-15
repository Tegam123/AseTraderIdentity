using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpaca.Markets;
using AseTrader.Models.Alpaca_dependency;
using RestSharp;
using RestClient = RestSharp.RestClient;


namespace AseTrader.Models.SellStock
{
    public class SellStock : ISellStock
    {
        public async Task SellStocks(string sym, int quantity, decimal price, IAlpacaClient alpaca)
        {
            await alpaca.AlpacaClientTrading().PostOrderAsync(sym, quantity, OrderSide.Sell, OrderType.Limit,
                TimeInForce.Day, price);
        }
        public void SellStocks_http(string sym, int quantity, decimal price, string accesstoken)
        {
            var client = new RestClient("https://paper-api.alpaca.markets/v2/orders");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {accesstoken}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", $"{{\r\n    \"symbol\": \"{sym}\", \r\n    \"qty\": {quantity},\r\n    \"side\": \"sell\",\r\n    \"type\": \"limit\",\r\n    \"time_in_force\": \"day\",\r\n    \"limit_price\": {price}\r\n}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);
        }
    }
}
