using NUnit.Framework;
using RestSharp;

namespace Unit_test_portfolio
{
    [TestFixture]
    public class portfolio_unit_test
    {
        private string accesstoken = "d8de6b5b-71c0-45e4-996d-83ba07cd28ec";

        [TestCase("TSLA")]
        [TestCase("SPY")]
        [TestCase("NFLX")]
        [TestCase("GOOGL")]
        [TestCase("AAPL")]
        public void test_correct_retrived_stocks(string symbol)
        {
            //arrange
            var client = new RestClient("https://paper-api.alpaca.markets/v2/positions");
            client.Timeout = -1;

            //act
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accesstoken}");
            IRestResponse response = client.Execute(request);

            string response_string = response.Content;

            //assert
            if (!response_string.Contains(symbol))
                Assert.Fail("Retrived portfolio failed. Not the correct stocks");
        }

        [TestCase("TSL")]
        [TestCase("WHS")]
        [TestCase("VTS")]
        [TestCase("FDD")]
        [TestCase("HGH")]
        public void test_wrong_symbols_not_in_portfolio(string symbol)
        {
            //arrange
            var client = new RestClient("https://paper-api.alpaca.markets/v2/positions");
            client.Timeout = -1;

            //act
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accesstoken}");
            IRestResponse response = client.Execute(request);

            string response_string = response.Content;

            //assert
            if (!response_string.Contains(symbol))
                Assert.Pass("The stocks searched are correct, not part of portfolio");
        }
    }
}