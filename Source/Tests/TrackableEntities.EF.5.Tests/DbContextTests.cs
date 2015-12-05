using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TrackableEntities.EF.Tests;
using TrackableEntities.EF.Tests.NorthwindModels;
using Xunit;

#if EF_6
namespace TrackableEntities.EF6.Tests
#else
namespace TrackableEntities.EF5.Tests
#endif
{
    public class DbContextTests
    {
        // Recreate database if model changed.
        // Specify CreateDbOptions.DropCreateDatabaseSeed to seed with real data.
        const CreateDbOptions CreateNorthwindDbOptions = CreateDbOptions.DropCreateDatabaseIfModelChanges;

        #region Update Parent State

        [Fact]
        public void Adding_Parent_Should_Mark_Children_As_Added()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductId = 1, CategoryId = 1, ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Added;

            // Assert
            Assert.Equal(EntityState.Added, context.Entry(product).State);
        }

        [Fact]
        public void Modifying_Parent_Should_Mark_Children_As_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductId = 1, CategoryId = 1, ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Modified;

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
        }

        [Fact]
        public void Deleting_Parent_And_Children_Should_Mark_Children_As_Deleted()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductId = 1, CategoryId = 1, ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryId = 1,
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(product).State = EntityState.Unchanged;
            context.Entry(category).State = EntityState.Unchanged;
            context.Entry(product).State = EntityState.Deleted;
            context.Entry(category).State = EntityState.Deleted;

            // Assert
            Assert.Equal(EntityState.Deleted, context.Entry(product).State);
        }

        #endregion

        #region Update Parent State And Save

        [Fact]
        public void Adding_Parent_Should_Add_Children()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Added;
            context.SaveChanges();

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
        }

        [Fact]
        public void Modifying_Parent_Should_Keep_Children_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };
            context.Categories.Add(category);
            context.SaveChanges();
            int categoryId = category.CategoryId;
            int productId = product.ProductId;
            context.Entry(product).State = EntityState.Detached;
            context.Entry(category).State = EntityState.Detached;
            product = new Product { ProductId = productId, CategoryId = categoryId, ProductName = "TestProduct" };
            category = new Category
            {
                CategoryId = categoryId,
                CategoryName = "TestCategory_Changed",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(product).State = EntityState.Unchanged;
            context.Entry(category).State = EntityState.Modified;
            context.SaveChanges();

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
        }

        [Fact]
        public void Deleting_Parent_Should_Delete_Children()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct"};
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };
            context.Categories.Add(category);
            context.SaveChanges();
            int categoryId = category.CategoryId;
            int productId = product.ProductId;
            context.Entry(product).State = EntityState.Detached;
            context.Entry(category).State = EntityState.Detached;
            product = new Product { ProductId = productId, CategoryId = categoryId, ProductName = "TestProduct" };
            category = new Category
            {
                CategoryId = categoryId,
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            // First mark child and parent as unchanged (to attach)
            context.Entry(product).State = EntityState.Unchanged;
            context.Entry(category).State = EntityState.Unchanged;

            // Then mark child and parent as deleted
            context.Entry(product).State = EntityState.Deleted;
            context.Entry(category).State = EntityState.Deleted;
            context.SaveChanges();

            // Assert
            Assert.Equal(EntityState.Detached, context.Entry(category).State);
            Assert.Equal(EntityState.Detached, context.Entry(product).State);
        }

        #endregion

        #region Update Child State

        [Fact]
        public void Adding_Child_To_Unchanged_Parent_Should_Keep_Parent_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Unchanged;
            context.Entry(product).State = EntityState.Added;

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
        }

        [Fact]
        public void Adding_Child_To_Modified_Parent_Should_Keep_Parent_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Modified;
            context.Entry(product).State = EntityState.Added;

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(category).State);
        }

        [Fact]
        public void Modified_Child_From_Unchanged_Parent_Should_Keep_Parent_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Unchanged;
            context.Entry(product).State = EntityState.Modified;

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
        }

        [Fact]
        public void Modified_Child_From_Modified_Parent_Should_Keep_Parent_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Modified;
            context.Entry(product).State = EntityState.Modified;

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(category).State);
        }

        [Fact]
        public void Deleting_Child_From_Unchanged_Parent_Should_Keep_Parent_Unchanged()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Unchanged;
            context.Entry(product).State = EntityState.Deleted;

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
        }

        [Fact]
        public void Deleting_Child_From_Modified_Parent_Should_Keep_Parent_Modified()
        {
            // Arrange
            var context = TestsHelper.CreateNorthwindDbContext(CreateNorthwindDbOptions);
            var product = new Product { ProductName = "TestProduct" };
            var category = new Category
            {
                CategoryName = "TestCategory",
                Products = new List<Product> { product }
            };

            // Act
            context.Entry(category).State = EntityState.Modified;
            context.Entry(product).State = EntityState.Deleted;

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(category).State);
        }

        #endregion
    }
}
