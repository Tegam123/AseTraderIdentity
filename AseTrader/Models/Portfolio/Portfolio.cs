using Newtonsoft.Json;
using RestSharp;

namespace AseTrader.Models.Portfolio
{
    public class Portfolio : IPortfolio
    {
        private string _pToken;

        public Portfolio(string accesstoken)
        {
            _pToken = accesstoken;
        }

        public PortfolioMapper SeePortfolio()
        {
            var client = new RestClient("https://paper-api.alpaca.markets/v2/positions");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {_pToken}");
            IRestResponse response = client.Execute(request);
            var JsonObj_PortfolioData = JsonConvert.DeserializeObject(response.Content);

            PortfolioMapper mapper = new PortfolioMapper();

            mapper.TradingInfo = JsonObj_PortfolioData;

            return mapper;
        }
    }
}