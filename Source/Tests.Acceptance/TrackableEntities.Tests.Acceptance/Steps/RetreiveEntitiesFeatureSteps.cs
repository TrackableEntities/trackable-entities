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
            var custIds = new List<string>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                string custName = row["CustomerName"];
                TestsHelper.EnsureTestCustomer(custId, custName);
                custIds.Add(custId);
            }
            ScenarioContext.Current.Add("CustIds", custIds);
        }

        [Given(@"the following orders")]
        public void GivenTheFollowingOrders(Table table)
        {
            var custOrders = new List<Order>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                var custOrder = TestsHelper.CreateTestOrder(custId);
                custOrders.Add(custOrder);
            }
            ScenarioContext.Current.Add("CustOrders", custOrders);
        }

        [When(@"I submit a GET request for customers")]
        public void WhenISubmitGetRequestForCustomers()
        {
            const string request = "api/Customer";
            var response = _client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.ToList();
            ScenarioContext.Current.Add("CustomersResult", result);
        }

        [When(@"I submit a GET request for customer orders")]
        public void WhenISubmitGetRequestForCustomerOrders()
        {
            var order = ScenarioContext.Current.Get<List<Order>>("CustOrders").First();
            string request = "api/Order?customerId=" + order.CustomerId;
            var response = _client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<Order>>()
                .Result.ToList();
            ScenarioContext.Current.Add("CustomerOrdersResult", result);
        }

        [Then(@"the request should return the customers")]
        public void ThenTheRequestShouldReturnTheCustomers()
        {
            var custId1 = ScenarioContext.Current.Get<List<string>>("CustIds")[0];
            var custId2 = ScenarioContext.Current.Get<List<string>>("CustIds")[1];
            var result = ScenarioContext.Current.Get<List<Customer>>("CustomersResult");
            Assert.IsTrue(result.Any(c => c.CustomerId == custId1));
            Assert.IsTrue(result.Any(c => c.CustomerId == custId2));
        }

        [Then(@"the request should return the orders")]
        public void ThenTheRequestShouldReturnTheOrders()
        {
            var orderId = ScenarioContext.Current.Get<List<Order>>("CustOrders")[0].OrderId;
            var result = ScenarioContext.Current.Get<List<Order>>("CustomerOrdersResult");
            Assert.IsTrue(result.Any(o => o.OrderId == orderId));
        }
    }
}
