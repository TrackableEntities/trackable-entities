#if EF_6
using System.Data.Entity;
#else
using System.Data;
#endif
using System.Collections.Generic;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.Mocks;
using TrackableEntities.EF.Tests.NorthwindModels;
using Xunit;

// ReSharper disable CheckNamespace
#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    public class StateChangeInterceptorTests
    {
        // ReSharper disable once InconsistentNaming
        private const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        #region No Relationship (Product)

        [Fact]
        public void WithStateChangeInterceptor_Should_Not_Mutate_Pool()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product();

            bool calledX = false, calledY = false;
            var poolX = context.WithStateChangeInterceptor<Product>((e, r) => { calledX = true; return null; });
            var poolXY = poolX.WithStateChangeInterceptor<Product>((e, r) => { calledY = true; return null; });

            // Act
            poolX.ApplyChanges(product);

            // Assert
            Assert.True(calledX);
            Assert.False(calledY);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Change_Single_Entity_State(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product();
            product.TrackingState = TrackingState.Modified;

            // Act
            var state = setState ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<Product>((e, r) => state)
                .ApplyChanges(product);

            // Assert
            var expectedState = setState ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(product).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_Multiple_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var products = new List<Product>
            {
                new Product { ProductId = 1, TrackingState = TrackingState.Added },
                new Product { ProductId = 2, TrackingState = TrackingState.Deleted }
            };

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Modified : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Product>((e, r) =>
                {
                    if (e.ProductId == 1)
                        return states[0];
                    if (e.ProductId == 2)
                        return states[1];
                    return null;
                })
                .ApplyChanges(products);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Modified : EntityState.Added,
                setState2 ? EntityState.Unchanged : EntityState.Deleted,
            };
            Assert.Equal(expectedStates[0], context.Entry(products[0]).State);
            Assert.Equal(expectedStates[1], context.Entry(products[1]).State);
        }

        #endregion

        #region 1-1 Relationship (Customer - CustomerSetting)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_One_To_One_Entity_State(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var customer = nw.Customers[0];
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
            setting.TrackingState = TrackingState.Modified;

            // Act
            var state = setState ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => state)
                .ApplyChanges(customer);

            // Assert
            var expectedState = setState ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(setting).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_One_To_One_MultiLevel_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var customer = nw.Customers[0];
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
            customer.TrackingState = TrackingState.Modified;
            setting.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Customer>((e, r) => states[0])
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => states[1])
                .ApplyChanges(customer);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(customer).State);
            Assert.Equal(expectedStates[1], context.Entry(setting).State);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_One_To_One_Entity_State_With_Added_Parent(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var customer = nw.Customers[0];
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
            customer.TrackingState = TrackingState.Added;

            // Act
            var state = setState ? EntityState.Modified : null as EntityState?;
            context
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => state)
                .ApplyChanges(customer);

            // Assert
            var expectedState = setState ? EntityState.Modified : EntityState.Added;
            Assert.Equal(expectedState, context.Entry(setting).State);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_One_To_One_Entity_State_With_Deleted_Parent(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var customer = nw.Customers[0];
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
            customer.TrackingState = TrackingState.Deleted;

            // Act
            var state = setState ? EntityState.Modified : null as EntityState?;
            context
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => state)
                .ApplyChanges(customer);

            // Assert
            var expectedState = setState ? EntityState.Modified : EntityState.Deleted;
            Assert.Equal(expectedState, context.Entry(setting).State);
        }

        [Theory]
        [InlineData(RelationshipType.OneToOne)]
        [InlineData(null)]
        public void State_Interceptor_Should_Mark_One_To_One_Entity_State_With_RelationType(RelationshipType? relationType)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var customer = nw.Customers[0];
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
            setting.TrackingState = TrackingState.Modified;

            // Act
            var state = relationType == RelationshipType.OneToOne ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => 
                    r == RelationshipType.OneToOne ? state : null)
                .ApplyChanges(customer);

            // Assert
            var expectedState = relationType == RelationshipType.OneToOne ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(setting).State);
        }

        #endregion

        #region 1-M Relationship (Order - OrderDetail)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_One_To_Many_Entity_State(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            detail1.TrackingState = TrackingState.Modified;

            // Act
            var state = setState ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<OrderDetail>((e, r) => state)
                .ApplyChanges(order);

            // Assert
            var expectedState = setState ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(detail1).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_One_To_Many_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var detail2 = order.OrderDetails[1];
            detail1.TrackingState = TrackingState.Modified;
            detail2.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<OrderDetail>((e, r) =>
                {
                    if (e.OrderDetailId == 1)
                        return states[0];
                    if (e.OrderDetailId == 2)
                        return states[1];
                    return null;
                })
                .ApplyChanges(order);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(detail1).State);
            Assert.Equal(expectedStates[1], context.Entry(detail2).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_One_To_Many_MultiLevel_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            order.TrackingState = TrackingState.Modified;
            detail1.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Order>((e, r) => states[0])
                .WithStateChangeInterceptor<OrderDetail>((e, r) => states[1])
                .ApplyChanges(order);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(order).State);
            Assert.Equal(expectedStates[1], context.Entry(detail1).State);
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void State_Interceptor_Should_Mark_One_To_Many_To_One_MultiLevel_Entities_States(
            bool setState1, bool setState2, bool setState3)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            var product = order.OrderDetails[0].Product;
            order.TrackingState = TrackingState.Modified;
            detail1.TrackingState = TrackingState.Modified;
            product.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
                setState3 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Order>((e, r) => states[0])
                .WithStateChangeInterceptor<OrderDetail>((e, r) => states[1])
                .WithStateChangeInterceptor<Product>((e, r) => states[2])
                .ApplyChanges(order);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
                setState3 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(order).State);
            Assert.Equal(expectedStates[1], context.Entry(detail1).State);
            Assert.Equal(expectedStates[2], context.Entry(product).State);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_One_To_Many_Entity_State_With_Added_Parent(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            order.TrackingState = TrackingState.Added;

            // Act
            var state = setState ? EntityState.Modified : null as EntityState?;
            context
                .WithStateChangeInterceptor<OrderDetail>((e, r) => state)
                .ApplyChanges(order);

            // Assert
            var expectedState = setState ? EntityState.Modified : EntityState.Added;
            Assert.Equal(expectedState, context.Entry(detail1).State);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_One_To_Many_Entity_State_With_Deleted_Parent(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            order.TrackingState = TrackingState.Deleted;

            // Act
            var state = setState ? EntityState.Modified : null as EntityState?;
            context
                .WithStateChangeInterceptor<OrderDetail>((e, r) => state)
                .ApplyChanges(order);

            // Assert
            var expectedState = setState ? EntityState.Modified : EntityState.Deleted;
            Assert.Equal(expectedState, context.Entry(detail1).State);
        }

        [Theory]
        [InlineData(RelationshipType.OneToMany)]
        [InlineData(null)]
        public void State_Interceptor_Should_Mark_One_To_Many_Entity_State_With_RelationType(RelationshipType? relationType)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var order = new MockNorthwind().Orders[0];
            var detail1 = order.OrderDetails[0];
            detail1.TrackingState = TrackingState.Modified;

            // Act
            var state = relationType == RelationshipType.OneToMany ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<OrderDetail>((e, r) =>
                    r == RelationshipType.OneToMany ? state : null)
                .ApplyChanges(order);

            // Assert
            var expectedState = relationType == RelationshipType.OneToMany ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(detail1).State);
        }

        #endregion

        #region M-1 Relationship (Order - Customer)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_Many_To_One_Entity_State(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var order = nw.Orders[0];
            var customer = order.Customer;
            customer.TrackingState = TrackingState.Modified;

            // Act
            var state = setState ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<Customer>((e, r) => state)
                .ApplyChanges(order);

            // Assert
            var expectedState = setState ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(customer).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_Many_To_One_MultiLevel_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var order = nw.Orders[0];
            var customer = order.Customer;
            order.TrackingState = TrackingState.Modified;
            customer.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Order>((e, r) => states[0])
                .WithStateChangeInterceptor<Customer>((e, r) => states[1])
                .ApplyChanges(order);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(order).State);
            Assert.Equal(expectedStates[1], context.Entry(customer).State);
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void State_Interceptor_Should_Mark_Many_To_One_To_One_MultiLevel_Entities_States(
            bool setState1, bool setState2, bool setState3)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var order = nw.Orders[0];
            var customer = order.Customer;
            var setting = customer.CustomerSetting = new CustomerSetting { CustomerId = customer.CustomerId, Setting = "Setting1", Customer = customer };
            order.TrackingState = TrackingState.Modified;
            customer.TrackingState = TrackingState.Modified;
            setting.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
                setState3 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Order>((e, r) => states[0])
                .WithStateChangeInterceptor<Customer>((e, r) => states[1])
                .WithStateChangeInterceptor<CustomerSetting>((e, r) => states[2])
                .ApplyChanges(order);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
                setState3 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(order).State);
            Assert.Equal(expectedStates[1], context.Entry(customer).State);
            Assert.Equal(expectedStates[2], context.Entry(setting).State);
        }

        [Theory]
        [InlineData(RelationshipType.ManyToOne)]
        [InlineData(null)]
        public void State_Interceptor_Should_Mark_Many_To_One_Entity_State_With_RelationType(RelationshipType? relationType)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var order = nw.Orders[0];
            var customer = order.Customer;
            customer.TrackingState = TrackingState.Modified;

            // Act
            var state = relationType == RelationshipType.ManyToOne ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<Customer>((e, r) =>
                    r == RelationshipType.ManyToOne ? state : null)
                .ApplyChanges(order);

            // Assert
            var expectedState = relationType == RelationshipType.ManyToOne ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(customer).State);
        }

        #endregion

        #region M-M Relationship (Employees - Territories)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_Many_To_Many_Entity_State(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            employee.Territories = new List<Territory> { territory1 };
            territory1.TrackingState = TrackingState.Modified;

            // Act
            var state = setState ? EntityState.Unchanged : null as EntityState?;
            context
                .WithStateChangeInterceptor<Territory>((e, r) => state)
                .ApplyChanges(employee);

            // Assert
            var expectedState = setState ? EntityState.Unchanged : EntityState.Modified;
            Assert.Equal(expectedState, context.Entry(territory1).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_Many_To_Many_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            var territory2 = employee.Territories[1];
            employee.Territories = new List<Territory> { territory1, territory2 };
            territory1.TrackingState = TrackingState.Modified;
            territory2.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Territory>((e, r) =>
                {
                    if (e.TerritoryId == "01581")
                        return states[0];
                    if (e.TerritoryId == "01730")
                        return states[1];
                    return null;
                })
                .ApplyChanges(employee);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(territory1).State);
            Assert.Equal(expectedStates[1], context.Entry(territory2).State);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void State_Interceptor_Should_Mark_Many_To_Many_MultiLevel_Entities_States(bool setState1, bool setState2)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            employee.Territories = new List<Territory> { territory1 };
            employee.TrackingState = TrackingState.Modified;
            territory1.TrackingState = TrackingState.Modified;

            // Act
            var states = new[]
            {
                setState1 ? EntityState.Unchanged : null as EntityState?,
                setState2 ? EntityState.Unchanged : null as EntityState?,
            };
            context
                .WithStateChangeInterceptor<Employee>((e, r) => states[0])
                .WithStateChangeInterceptor<Territory>((e, r) => states[1])
                .ApplyChanges(employee);

            // Assert
            var expectedStates = new[]
            {
                setState1 ? EntityState.Unchanged : EntityState.Modified,
                setState2 ? EntityState.Unchanged : EntityState.Modified,
            };
            Assert.Equal(expectedStates[0], context.Entry(employee).State);
            Assert.Equal(expectedStates[1], context.Entry(territory1).State);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void State_Interceptor_Should_Mark_Many_To_Many_Entity_State_With_Added_Child(bool setState)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            employee.Territories = new List<Territory> { territory1 };
            employee.TrackingState = TrackingState.Unchanged;
            territory1.TrackingState = TrackingState.Added;

            // Act
            var state = setState ? EntityState.Added : null as EntityState?;
            context
                .WithStateChangeInterceptor<Territory>((e, r) => state)
                .ApplyChanges(employee);

            // Assert
            var expectedState = setState ? EntityState.Added : EntityState.Unchanged;
            Assert.Equal(expectedState, context.Entry(territory1).State);
        }

        [Theory]
        [InlineData(RelationshipType.ManyToMany)]
        [InlineData(null)]
        public void State_Interceptor_Should_Mark_Many_To_Many_Entity_State_With_RelationType(RelationshipType? relationType)
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var nw = new MockNorthwind();
            var employee = nw.Employees[0];
            var territory1 = employee.Territories[0];
            employee.Territories = new List<Territory> { territory1 };
            employee.TrackingState = TrackingState.Unchanged;
            territory1.TrackingState = TrackingState.Added;

            // Act
            var state = relationType == RelationshipType.ManyToMany ? EntityState.Added : null as EntityState?;
            context
                .WithStateChangeInterceptor<Territory>((e, r) =>
                    r == RelationshipType.ManyToMany ? state : null)
                .ApplyChanges(employee);

            // Assert
            var expectedState = relationType == RelationshipType.ManyToMany ? EntityState.Added : EntityState.Unchanged;
            Assert.Equal(expectedState, context.Entry(territory1).State);
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
