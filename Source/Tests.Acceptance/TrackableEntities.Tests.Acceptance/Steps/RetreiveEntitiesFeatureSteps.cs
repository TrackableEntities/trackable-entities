using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.SelfHost;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TrackableEntities.EF.Tests.NorthwindModels;
using TrackableEntities.Tests.Acceptance.Helpers;
using TrackableEntities.Tests.Acceptance.WebHost;

namespace TrackableEntities.Tests.Acceptance.Steps
{
    [Binding]
    public class RetreiveEntitiesFeatureSteps
    {
        private HttpClient _client;
        private HttpSelfHostServer _server;

        private string _custId1;
        private string _custName1;
        private string _custId2;
        private string _custName2;
        private List<Customer> _result;


        [BeforeScenario]
        public void Setup()
        {
            _client = WebHostClient.Create();
            _server = WebHostServer.Create();
            _server.OpenAsync().Wait();
        }

        [AfterScenario]
        public void TearDown()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
            _client.Dispose();
        }

        [Given(@"the following customers")]
        public void GivenTheFollowingCustomers(Table table)
        {
            _custId1 = table.Rows[0]["CustomerId"];
            _custName1 = table.Rows[0]["CustomerName"];
            TestsHelper.EnsureTestCustomer(_custId1, _custName1);

            _custId2 = table.Rows[1]["CustomerId"];
            _custName2 = table.Rows[1]["CustomerName"];
            TestsHelper.EnsureTestCustomer(_custId2, _custName2);
        }

        [When(@"I submit a GET request for customers")]
        public void WhenISubmitGetRequestForCustomers()
        {
            const string request = "api/Customer";
            var response = _client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Customer>>().Result;
            _result = new List<Customer>(result);
        }

        [Then(@"the request should return the customers")]
        public void ThenTheRequestShouldReturnTheCustomers()
        {
            var customer1 = _result.SingleOrDefault(c => c.CustomerId == _custId1);
            var customer2 = _result.SingleOrDefault(c => c.CustomerId == _custId2);
            Assert.NotNull(customer1);
            Assert.NotNull(customer2);
        }
    }
}
