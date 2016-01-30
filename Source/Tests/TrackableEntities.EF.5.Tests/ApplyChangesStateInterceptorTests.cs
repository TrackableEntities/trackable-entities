using System.Collections.Generic;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF.Tests.NorthwindModels;
using TrackableEntities.EF.Tests.TestData.StateInterceptor;
using Xunit;
#if EF_6
using System.Data.Entity;
#else
using System.Data;
#endif

// ReSharper disable CheckNamespace
#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    public class ApplyChangesStateInterceptorTests
    {
        // ReSharper disable once InconsistentNaming
        private const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        #region No Relationship (Product)

        [Theory]
        [ClassData(typeof(SingleStateConfigGenerated))]
        public void State_Interceptor_Should_Mark_Single_Entity_State (StateConfig stateConfig)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product();
            product.TrackingState = stateConfig.InitState;

            // Act
            context
                .WithStateChangeInterceptor<Product>((e, rs) => stateConfig.UseInterceptor ? stateConfig.FinalState : (EntityState?)null)
                .ApplyChanges(product);

            // Assert
            Assert.Equal(stateConfig.FinalState, context.Entry(product).State);
        }

        [Theory]
        [ClassData(typeof(TwoStateConfigsGenerated))]
        public void State_Interceptor_Should_Mark_Multiple_Entities_States (StateConfig stateConfig1, StateConfig stateConfig2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var products = new List<Product>
            {
                new Product { ProductId = 1, TrackingState = stateConfig1.InitState },
                new Product { ProductId = 2, TrackingState = stateConfig2.InitState  }
            };

            // Act
            context
                .WithStateChangeInterceptor<Product>((e, rs) =>
                {
                    if (e.ProductId == 1 && stateConfig1.UseInterceptor)
                        return stateConfig1.FinalState;

                    if (e.ProductId == 2 && stateConfig2.UseInterceptor)
                        return stateConfig2.FinalState;

                    return null;
                })
                .ApplyChanges(products);

            // Assert
            Assert.Equal(stateConfig1.FinalState, context.Entry(products[0]).State);
            Assert.Equal(stateConfig2.FinalState, context.Entry(products[1]).State);
        }
        #endregion

        #region 1-1 Relationship (Customer - CustomerSetting)
        [Theory]
        [ClassData(typeof(TwoStateConfigsGenerated))]
        public void State_Interceptor_Should_Mark_One_To_One_Entities_States (StateConfig customerConfig, StateConfig settingConfig)
        {
            // NOTE: customer.CustomerSetting will be set to null if customer is deleted.

            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var customer = nw.Customers[0];
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };

            customer.TrackingState = customerConfig.InitState;
            setting.TrackingState = settingConfig.InitState;

            // Act
            context
                .WithStateChangeInterceptor<Customer>((e, r) => customerConfig.UseInterceptor ? customerConfig.FinalState : (EntityState?)null)
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => settingConfig.UseInterceptor ? settingConfig.FinalState : (EntityState?)null)
                .ApplyChanges(customer);

            // Assert
            Assert.Equal(customerConfig.FinalState, context.Entry(customer).State);
            Assert.Equal(settingConfig.FinalState, context.Entry(setting).State);
        }
        #endregion

        #region 1-M Relationship (Order - OrderDetail)
        [Theory]
        [ClassData(typeof(TwoStateConfigsGenerated))]
        public void State_Interceptor_Should_Mark_One_To_Many_Entities_States(StateConfig orderConfig, StateConfig detailsConfig)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];

            order.TrackingState = orderConfig.InitState;
            detail1.TrackingState = detailsConfig.InitState;
            detail2.TrackingState = detailsConfig.InitState;

            // Act
            context
                .WithStateChangeInterceptor<Order>((e, r) => orderConfig.UseInterceptor ? orderConfig.FinalState : (EntityState?)null)
                .WithStateChangeInterceptor<OrderDetail>((e, r) => detailsConfig.UseInterceptor ? detailsConfig.FinalState : (EntityState?)null)
                .ApplyChanges(order);

            // Assert
            Assert.Equal(orderConfig.FinalState, context.Entry(order).State);
            Assert.Equal(detailsConfig.FinalState, context.Entry(detail1).State);
            Assert.Equal(detailsConfig.FinalState, context.Entry(detail2).State);
        }
        #endregion

        #region M-1 Relationship (Order - Customer)
        [Theory]
        [ClassData(typeof(TwoStateConfigsGenerated))]
        public void State_Interceptor_Should_Mark_Many_To_One_Entities_States(StateConfig orderConfig, StateConfig customerConfig)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var order = nw.Orders[0];
            var customer = order.Customer;

            order.TrackingState = orderConfig.InitState;
            customer.TrackingState = customerConfig.InitState;

            // Act
            context
                .WithStateChangeInterceptor<Order>((e, r) => orderConfig.UseInterceptor ? orderConfig.FinalState : (EntityState?)null)
                .WithStateChangeInterceptor<Customer>((e, r) => customerConfig.UseInterceptor ? customerConfig.FinalState : (EntityState?)null)
                .ApplyChanges(order);

            // Assert
            Assert.Equal(orderConfig.FinalState, context.Entry(order).State);
            Assert.Equal(customerConfig.FinalState, context.Entry(customer).State);
        }
        #endregion

        #region M-M Relationship (Employees - Territories)
        [Theory]
        [ClassData(typeof(TwoStateConfigsGenerated))]
        public void State_Interceptor_Should_Mark_Many_To_Many_Entities_States(StateConfig employeeConfig, StateConfig territoryConfig)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            
            employee.TrackingState = employeeConfig.InitState;
            territory1.TrackingState = territoryConfig.InitState;

            // Act
            context
                .WithStateChangeInterceptor<Employee>((e, r) => employeeConfig.UseInterceptor ? employeeConfig.FinalState : (EntityState?)null)
                .WithStateChangeInterceptor<Territory>((e, r) => territoryConfig.UseInterceptor ? territoryConfig.FinalState : (EntityState?)null)
                .ApplyChanges(employee);

            // Assert
            Assert.Equal(employeeConfig.FinalState, context.Entry(employee).State);
            Assert.Equal(territoryConfig.FinalState, context.Entry(territory1).State);
        }
        #endregion

        #region Multiple State Interceptors

        [Fact]
        public void Last_Interceptor_Should_Win()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            var detail3 = order.OrderDetails[2];

            order.TrackingState = TrackingState.Unchanged;
            detail1.TrackingState = TrackingState.Unchanged;
            detail2.TrackingState = TrackingState.Unchanged;
            detail3.TrackingState = TrackingState.Unchanged;

            var orderId = order.OrderId;
            var detailId1 = detail1.OrderDetailId;
            var detailId2 = detail2.OrderDetailId;
            var detailId3 = detail3.OrderDetailId;

            // Act
            context
                // Order -> Deleted
                .WithStateChangeInterceptor<Order>((e, rs) =>
                {
                    if (e.OrderId == orderId)
                        return EntityState.Deleted;

                    return null;
                })
                // OrderDetail 1 -> Added   <====== FINAL STATE
                // OrderDetail 2 -> Modidfied
                // OrderDetail 3 -> Deleted
                .WithStateChangeInterceptor<OrderDetail>((e, rs) =>
                {
                    if (e.OrderDetailId == detailId1)
                        return EntityState.Added;

                    if (e.OrderDetailId == detailId2)
                        return EntityState.Modified;

                    if (e.OrderDetailId == detailId3)
                        return EntityState.Deleted;

                    return null;
                })
                // Order -> Modified   <====== FINAL STATE
                .WithStateChangeInterceptor<Order>((e, rs) =>
                {
                    if (e.OrderId == orderId)
                        return EntityState.Modified;

                    return null;
                })
                // OrderDetail 2 -> Added   <====== FINAL STATE
                .WithStateChangeInterceptor<OrderDetail>((e, rs) =>
                {
                    if (e.OrderDetailId == detailId2)
                        return EntityState.Added;

                    return null;
                })
                // OrderDetail 3 -> Modified   <====== FINAL STATE
                .WithStateChangeInterceptor<OrderDetail>((e, rs) =>
                {
                    if (e.OrderDetailId == detailId3)
                        return EntityState.Modified;

                    return null;
                })
                .ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(order).State);
            Assert.Equal(EntityState.Added, context.Entry(detail1).State);
            Assert.Equal(EntityState.Added, context.Entry(detail2).State);
            Assert.Equal(EntityState.Modified, context.Entry(detail3).State);
        }

        #endregion
    }
}
