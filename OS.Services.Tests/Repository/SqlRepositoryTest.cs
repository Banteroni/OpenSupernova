using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using OS.Data.Context;
using OS.Data.Models;
using OS.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Tests.Repository
{
    public class SqlRepositoryTest
    {
        [Test]
        public async Task CreateAsyncTest()
        {

            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();

                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;

                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    // Act
                    var result = await repository.CreateAsync(entity);
                    // Assert
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result, Is.EqualTo(entity));
                    Assert.That(result.Name, Is.EqualTo(entity.Name));

                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task DeleteAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var deleted = await repository.DeleteAsync<Artist>(result.Id);
                    // Assert
                    Assert.That(deleted, Is.Not.Null);
                    Assert.That(deleted, Is.EqualTo(entity));
                    Assert.That(deleted?.Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task DeleteAsyncTest_IdNotFound()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var deleted = await repository.DeleteAsync<Artist>(Guid.NewGuid());
                    // Assert
                    Assert.That(deleted, Is.Null);
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task DeleteWhereAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var deleted = await repository.DeleteWhereAsync<Artist>(a => a.Name == "Test Artist");
                    // Assert
                    Assert.That(deleted, Is.Not.Null);
                    Assert.That(deleted, Is.Not.Empty);
                    Assert.That(deleted.Count(), Is.EqualTo(1));
                    Assert.That(deleted.First(), Is.EqualTo(entity));
                    Assert.That(deleted.First().Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task DeleteWhereAsyncTest_NoEntitiesFound()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var deleted = await repository.DeleteWhereAsync<Artist>(a => a.Name == "Not Found");
                    // Assert
                    Assert.That(deleted, Is.Not.Null);
                    Assert.That(deleted, Is.Empty);
                    await connection.CloseAsync();
                }
            }
        }

        [Test]
        public async Task FindAllAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var found = await repository.FindAllAsync<Artist>(a => a.Name == "Test Artist", [nameof(Artist.Albums)]);
                    // Assert
                    Assert.That(found, Is.Not.Null);
                    Assert.That(found, Is.Not.Empty);
                    Assert.That(found.Count(), Is.EqualTo(1));
                    Assert.That(found.First(), Is.EqualTo(entity));
                    Assert.That(found.First().Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task FindFirstAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var found = await repository.FindFirstAsync<Artist>(a => a.Name == "Test Artist", [nameof(Artist.Albums)]);
                    // Assert
                    Assert.That(found, Is.Not.Null);
                    Assert.That(found, Is.EqualTo(entity));
                    Assert.That(found.Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task GetAllAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var found = await repository.GetAllAsync<Artist>([nameof(Artist.Albums)]);
                    // Assert
                    Assert.That(found, Is.Not.Null);
                    Assert.That(found, Is.Not.Empty);
                    Assert.That(found.Count(), Is.EqualTo(2)); // Unknown Artist included
                    Assert.That(found.First(x => x.Id == entity.Id), Is.EqualTo(entity));
                    Assert.That(found.First(x => x.Id == entity.Id).Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task GetAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    // Act
                    var found = await repository.GetAsync<Artist>(entity.Id, [nameof(Artist.Albums)]);
                    // Assert
                    Assert.That(found, Is.Not.Null);
                    Assert.That(found, Is.EqualTo(entity));
                    Assert.That(found.Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
        [Test]
        public async Task SaveChangesAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    entity.Name = "Updated Artist";
                    // Act
                    var updated = await repository.SaveChangesAsync();
                    // Assert
                    Assert.That(updated, Is.EqualTo(true));
                    await connection.CloseAsync();
                }
            }
        }

        [Test]
        public async Task UpdateAsyncTest()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                // Arrange
                await connection.OpenAsync();
                var options = new DbContextOptionsBuilder<OsDbContext>()
                    .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                using (var context = new OsDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                    var repository = new SqlRepository(context);
                    var entity = new Artist()
                    {
                        Name = "Test Artist"
                    };
                    var result = await repository.CreateAsync(entity);
                    entity.Name = "Updated Artist";
                    // Act
                    var updated = await repository.UpdateAsync(entity);
                    // Assert
                    Assert.That(updated, Is.Not.Null);
                    Assert.That(updated, Is.EqualTo(entity));
                    Assert.That(updated.Name, Is.EqualTo(entity.Name));
                    await connection.CloseAsync();
                }
            }
        }
    }
}
