using NUnit.Framework;
using RestSharp;

namespace Unit_test_buystock
{
    public class buystock_unit_test
    {
        [TestFixture]
        private class NUnit_test_buystock
        {
            private string accesstoken = "d8de6b5b-71c0-45e4-996d-83ba07cd28ec";
            private dynamic obj = "Test";

            [TestCase("AAPL", 2, 20)]
            public void test_BuyStock_With_Accepted_values(string sym, int quan, decimal price)
            {
                //Arrange
                var client = new RestClient("https://paper-api.alpaca.markets/v2/orders");
                var request = new RestRequest(Method.POST);

                //act
                request.AddHeader("Authorization", $"Bearer {accesstoken}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", $"{{\r\n    \"symbol\": \"{sym}\", \r\n    \"qty\": {quan},\r\n    \"side\": \"buy\",\r\n    \"type\": \"limit\",\r\n    \"time_in_force\": \"day\",\r\n    \"limit_price\": {price}\r\n}}", ParameterType.RequestBody);

                IRestResponse restResponse = client.Execute(request);
                string response_string = restResponse.Content;

                //Assert
                if (!response_string.Contains("accepted"))
                    Assert.Fail("The stock was not bought, try again");
            }

            [TestCase("AAPL2", 2, 20)]
            public void test_BuyStock_With_wrong_symbol(string sym, int quan, decimal price)
            {
                //Arrange
                var client = new RestClient("https://paper-api.alpaca.markets/v2/orders");
                var request = new RestRequest(Method.POST);

                //act
                request.AddHeader("Authorization", $"Bearer {accesstoken}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", $"{{\r\n    \"symbol\": \"{sym}\", \r\n    \"qty\": {quan},\r\n    \"side\": \"buy\",\r\n    \"type\": \"limit\",\r\n    \"time_in_force\": \"day\",\r\n    \"limit_price\": {price}\r\n}}", ParameterType.RequestBody);
                IRestResponse restResponse = client.Execute(request);

                string response_string = restResponse.Content;

                if (!response_string.Contains("40010001"))
                    Assert.Fail("The stock was not bought, try again");
            }
        }
    }
}