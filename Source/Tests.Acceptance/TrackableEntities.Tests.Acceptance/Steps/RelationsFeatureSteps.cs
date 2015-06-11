using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.SelfHost;
using TechTalk.SpecFlow;
using TrackableEntities.Client;
using TrackableEntities.Common;
using TrackableEntities.Tests.Acceptance.ClientEntities;
using TrackableEntities.Tests.Acceptance.Helpers;
using TrackableEntities.Tests.Acceptance.WebHost;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Steps
{
    [Binding]
    public class RelationsFeatureSteps
    {
        private HttpClient _client;
        private HttpSelfHostServer _server;
        private const int PortNumber = 56792;

        [BeforeScenario]
        public void Setup()
        {
            _client = WebHostClient.Create(PortNumber);
            _server = WebHostServer.Create(PortNumber);
            _server.OpenAsync().Wait();
        }

        [AfterScenario]
        public void TearDown()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
            _client.Dispose();
        }

        [Given(@"the following new customer")]
        public void GivenTheFollowingNewCustomer(Table table)
        {
            ScenarioContext.Current.Pending();

            string custId = table.Rows[0]["CustomerId"];
            string custName = table.Rows[0]["CustomerName"];
            var customer = new Customer
            {
                CustomerId = custId,
                CustomerName = custName
            };
            ScenarioContext.Current.Add("Customer", customer);
        }

        [When(@"I submit a POST to create a customer")]
        public void WhenISubmitPostToCreateAnEntity()
        {
            ScenarioContext.Current.Pending();

            var customer = ScenarioContext.Current.Get<Customer>("Customer");
            var changeTracker = new ChangeTrackingCollection<Customer>(true) { customer };
            Customer result = _client.CreateEntity(customer);
            changeTracker.MergeChanges(result);
        }

        [When(@"I add a customer setting")]
        public void WhenIAddACustomerSetting()
        {
            ScenarioContext.Current.Pending();

            var customer = ScenarioContext.Current.Get<Customer>("Customer");
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId,
                Setting = " Test Setting",
                TrackingState = TrackingState.Added // Mark as added
            };
        }

        [When(@"I submit a PUT to update the customer")]
        public void WhenISubmitPutToUpdateTheCustomer()
        {
            ScenarioContext.Current.Pending();

            var customer = ScenarioContext.Current.Get<Customer>("Customer");
            _client.UpdateTEntity(customer);
            customer.AcceptChanges();
        }
        
        [Then(@"the request should return the new customer")]
        public void ThenTheRequestShouldReturnTheNewCustomer()
        {
            ScenarioContext.Current.Pending();

            var customer = ScenarioContext.Current.Get<Customer>("Customer");
            Customer result = _client.GetEntity<Customer, string>(customer.CustomerId);
            Assert.NotNull(result);
            Assert.Equal(customer.CustomerName, result.CustomerName);
            Assert.Equal(customer.CustomerSetting.Setting, result.CustomerSetting.Setting);
        }
    }
}
