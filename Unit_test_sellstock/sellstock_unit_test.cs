using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace Unit_test_sellstock
{

    [TestFixture]
    public class sellstock_unit_test
    {
        private string accesstoken = "d8de6b5b-71c0-45e4-996d-83ba07cd28ec";


        [TestCase("WTS", 1, 420)]
        [TestCase("SPY", 2, 300)]
        [TestCase("GOOGL", 1, 700)]
        [TestCase("TSLA", 1, 110)]
        [TestCase("AAPL", 1, 510)]
        public void test_SellStock_With_Accepted_values_stock_is_owned(string sym, int quantity, decimal price)
        {
            //Arrange - Sells a stock
            var client = new RestClient("https://paper-api.alpaca.markets/v2/orders");
            var request = new RestRequest(Method.POST);

            //act
            request.AddHeader("Authorization", $"Bearer {accesstoken}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", $"{{\r\n    \"symbol\": \"{sym}\", \r\n    \"qty\": {quantity},\r\n    \"side\": \"sell\",\r\n    \"type\": \"limit\",\r\n    \"time_in_force\": \"day\",\r\n    \"limit_price\": {price}\r\n}}", ParameterType.RequestBody);

            IRestResponse restResponse = client.Execute(request);
            string response_string = restResponse.Content;


            //Assert
            if (!response_string.Contains("accepted"))
                Assert.Fail("The stock was not sold, try again");
        }

        [TestCase("AAPL2", 2, 260)]
        [TestCase("WTS2", 28, 2000)]
        [TestCase("HGL", 12, 230)]
        [TestCase("AAPLL", 6, 207)]
        public void test_SellStock_With_wrong_symbol(string sym, int quantity, decimal price)
        {
            //Arrange
            var client = new RestClient("https://paper-api.alpaca.markets/v2/orders");
            var request = new RestRequest(Method.POST);

            //act
            request.AddHeader("Authorization", $"Bearer {accesstoken}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", $"{{\r\n    \"symbol\": \"{sym}\", \r\n    \"qty\": {quantity},\r\n    \"side\": \"sell\",\r\n    \"type\": \"limit\",\r\n    \"time_in_force\": \"day\",\r\n    \"limit_price\": {price}\r\n}}", ParameterType.RequestBody);

            IRestResponse restResponse = client.Execute(request);
            string response_string = restResponse.Content;


            //Assert
            if (!response_string.Contains("40010001"))
                Assert.Fail("The stock was not sold, try again");
        }

    }
}
