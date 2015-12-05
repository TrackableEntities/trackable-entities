using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.SelfHost;
using TechTalk.SpecFlow;
using TrackableEntities.Client;
using TrackableEntities.EF.Tests.NorthwindModels;
using TrackableEntities.Tests.Acceptance.Helpers;
using TrackableEntities.Tests.Acceptance.WebHost;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Steps
{
    [Binding]
    public class BasicFeatureSteps
    {
        private HttpClient _client;
        private HttpSelfHostServer _server;
        private const int PortNumber = 56791;

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

        [Given(@"the following customer orders")]
        public void GivenTheFollowingOrders(Table table)
        {
            var orders = new List<Order>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                TestsHelper.EnsureTestCustomer(custId, "Test Customer " + custId);
                var order = TestsHelper.EnsureTestOrder(custId);
                orders.Add(order);
            }
            ScenarioContext.Current.Add("CustOrders", orders);
        }

        [Given(@"the following new customer orders")]
        public void GivenTheFollowingNewCustomerOrders(Table table)
        {
            var orders = new List<ClientEntities.Order>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                int[] productIds = TestsHelper.CreateTestProducts();
                ScenarioContext.Current.Add("ProductIds", productIds);
                var clientOrder = EntityExtensions.CreateNewOrder(custId, productIds);
                orders.Add(clientOrder);
            }
            ScenarioContext.Current.Add("NewCustOrders", orders);
        }

        [Given(@"the following existing customer orders")]
        public void GivenTheFollowingExistingCustomerOrders(Table table)
        {
            var orders = new List<ClientEntities.Order>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                TestsHelper.EnsureTestCustomer(custId, "Test Customer " + custId);
                int[] productIds = TestsHelper.CreateTestProducts();
                ScenarioContext.Current.Add("ProductIds", productIds);
                var order = TestsHelper.CreateTestOrder(custId, productIds);
                orders.Add(order.ToClientEntity());
            }
            ScenarioContext.Current.Add("ExistingCustOrders", orders);
        }

        [Given(@"the order is modified")]
        public void GivenTheOrderIsModified()
        {
            var order = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").First();
            var changeTracker = new ChangeTrackingCollection<ClientEntities.Order>(order);
            ScenarioContext.Current.Add("DeletedDetail", order.OrderDetails[1]);
            int[] productIds = ScenarioContext.Current.Get<int[]>("ProductIds");
            var addedDetail = new ClientEntities.OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = productIds[3],
                Quantity = 15,
                UnitPrice = 30
            };
            ScenarioContext.Current.Add("AddedDetail", addedDetail);
            order.OrderDate = order.OrderDate.AddDays(1);
            order.OrderDetails[0].UnitPrice++;
            order.OrderDetails.RemoveAt(1);
            order.OrderDetails.Add(addedDetail);
            ScenarioContext.Current.Add("ChangeTracker", changeTracker);
        }

        [Given(@"order details are added")]
        public void GivenOrderDetailsAreAdded()
        {
            var order = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").First();
            var changeTracker = new ChangeTrackingCollection<ClientEntities.Order>(order);
            int[] productIds = ScenarioContext.Current.Get<int[]>("ProductIds");
            var addedDetail1 = new ClientEntities.OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = productIds[3],
                Quantity = 15,
                UnitPrice = 30
            };
            var addedDetail2 = new ClientEntities.OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = productIds[4],
                Quantity = 20,
                UnitPrice = 40
            };
            ScenarioContext.Current.Add("AddedDetail1", addedDetail1);
            ScenarioContext.Current.Add("AddedDetail2", addedDetail2);
            order.OrderDetails.Add(addedDetail1);
            order.OrderDetails.Add(addedDetail2);
            ScenarioContext.Current.Add("ChangeTracker", changeTracker);
        }
        
        [When(@"I submit a GET request for customers")]
        public void WhenISubmitGetRequestForCustomers()
        {
            const string request = "api/Customer";
            var response = _client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<ClientEntities.Customer>>().Result.ToList();
            ScenarioContext.Current.Add("CustomersResult", result);
        }

        [When(@"I submit a GET request for customer orders")]
        public void WhenISubmitGetRequestForCustomerOrders()
        {
            var order = ScenarioContext.Current.Get<List<Order>>("CustOrders").First();
            string request = "api/Order?customerId=" + order.CustomerId;
            var response = _client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsAsync<IEnumerable<ClientEntities.Order>>().Result.ToList();
            ScenarioContext.Current.Add("CustomerOrdersResult", result);
        }

        [When(@"I submit a GET request for an order")]
        public void WhenISubmitGetRequestForAnOrder()
        {
            var order = ScenarioContext.Current.Get<List<Order>>("CustOrders").First();
            string request = "api/Order/" + order.OrderId;
            var response = _client.GetAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var orderResult = response.Content.ReadAsAsync<ClientEntities.Order>().Result;
            var result = new List<ClientEntities.Order> { orderResult };
            ScenarioContext.Current.Add("CustomerOrdersResult", result);
        }

        [When(@"I submit a POST to create an order")]
        public void WhenISubmitPostToCreateAnOrder()
        {
            var clientOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("NewCustOrders").First();
            string request = "api/Order";
            var response = _client.PostAsJsonAsync(request, clientOrder).Result;
            response.EnsureSuccessStatusCode();
            var orderResult = response.Content.ReadAsAsync<ClientEntities.Order>().Result;
            var result = new List<ClientEntities.Order> { orderResult };
            ScenarioContext.Current.Add("CustomerOrdersResult", result);
        }

        [When(@"I submit a PUT to modify an order")]
        public void WhenISubmitPutToModifyAnOrder()
        {
            var clientOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").First();
            var changeTracker = ScenarioContext.Current.Get<ChangeTrackingCollection<ClientEntities.Order>>("ChangeTracker");
            var clonedOrder = changeTracker.Clone()[0];
            ScenarioContext.Current["ExistingCustOrders"] = new List<ClientEntities.Order> { clonedOrder };
            var changedOrder = changeTracker.GetChanges().SingleOrDefault();

            string request = "api/Order";
            var response = _client.PutAsJsonAsync(request, changedOrder).Result;
            response.EnsureSuccessStatusCode();
            var orderResult = response.Content.ReadAsAsync<ClientEntities.Order>().Result;
            
            changeTracker.MergeChanges(orderResult);
            ScenarioContext.Current.Add("CustomerOrdersResult", new List<ClientEntities.Order>{ clientOrder});
        }

        [When(@"I submit a DELETE to delete an order")]
        public void WhenISubmitDeleteToDeleteAnOrder()
        {
            var clientOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").First();
            string request = "api/Order/" + clientOrder.OrderId;
            var response = _client.DeleteAsync(request);
            response.Result.EnsureSuccessStatusCode();
        }
        
        [Then(@"the request should return the customers")]
        public void ThenTheRequestShouldReturnTheCustomers()
        {
            var custId1 = ScenarioContext.Current.Get<List<string>>("CustIds")[0];
            var custId2 = ScenarioContext.Current.Get<List<string>>("CustIds")[1];
            var result = ScenarioContext.Current.Get<List<ClientEntities.Customer>>("CustomersResult");
            Assert.True(result.Any(c => c.CustomerId == custId1));
            Assert.True(result.Any(c => c.CustomerId == custId2));
        }

        [Then(@"the request should return the orders")]
        public void ThenTheRequestShouldReturnTheOrders()
        {
            var result = ScenarioContext.Current.Get<List<ClientEntities.Order>>("CustomerOrdersResult");
            var orders = ScenarioContext.Current.Get<List<Order>>("CustOrders");
            foreach (var order in orders)
            {
                Assert.True(result.Any(o => o.OrderId == order.OrderId));
            }
        }

        [Then(@"the request should return the new order")]
        public void ThenTheRequestShouldReturnTheNewOrders()
        {
            var result = ScenarioContext.Current.Get<List<ClientEntities.Order>>("CustomerOrdersResult").Single();
            Assert.True(result.OrderId > 0);
        }

        [Then(@"the request should return the modified order")]
        public void ThenTheRequestShouldReturnTheModifiedOrder()
        {
            var modifiedOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").Single();
            var updatedOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("CustomerOrdersResult").Single();
            var addedDetail = ScenarioContext.Current.Get<ClientEntities.OrderDetail>("AddedDetail");
            var deletedDetail = ScenarioContext.Current.Get<ClientEntities.OrderDetail>("DeletedDetail");

            Assert.Equal(modifiedOrder.OrderDate, updatedOrder.OrderDate);
            Assert.Equal(modifiedOrder.OrderDetails[0].UnitPrice, updatedOrder.OrderDetails[0].UnitPrice);
            Assert.True(updatedOrder.OrderDetails.Any(d => d.ProductId == addedDetail.ProductId));
            Assert.False(updatedOrder.OrderDetails.Any(d => d.ProductId == deletedDetail.ProductId));
        }

        [Then(@"the request should return the added order details")]
        public void ThenTheRequestShouldReturnTheAddedOrderDetails()
        {
            var modifiedOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").Single();
            var updatedOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("CustomerOrdersResult").Single();
            var addedDetail1 = ScenarioContext.Current.Get<ClientEntities.OrderDetail>("AddedDetail1");
            var addedDetail2 = ScenarioContext.Current.Get<ClientEntities.OrderDetail>("AddedDetail2");

            Assert.Equal(modifiedOrder.OrderDetails[0].UnitPrice, updatedOrder.OrderDetails[0].UnitPrice);
            Assert.True(updatedOrder.OrderDetails.Any(d => d.ProductId == addedDetail1.ProductId));
            Assert.True(updatedOrder.OrderDetails.Any(d => d.ProductId == addedDetail2.ProductId));
        }
        
        [Then(@"the order should be deleted")]
        public void ThenTheOrderShouldBeDeleted()
        {
            var clientOrder = ScenarioContext.Current.Get<List<ClientEntities.Order>>("ExistingCustOrders").First();
            string request = "api/Order/" + clientOrder.OrderId;
            var response = _client.GetAsync(request).Result;
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
