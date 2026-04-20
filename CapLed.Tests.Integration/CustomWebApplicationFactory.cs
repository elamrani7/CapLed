using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Infrastructure.Persistence;
using System.Linq;

namespace CapLed.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = System.Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing MySql DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StockManagementDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add EF In-Memory database
            services.AddDbContext<StockManagementDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            // Mock IEmailService
            var emailServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailServiceDescriptor != null) services.Remove(emailServiceDescriptor);
            
            var emailMock = new Mock<IEmailService>();
            services.AddSingleton<IEmailService>(emailMock.Object);

            // Mock IUnitOfWork for InMemory Database (since InMemory doesn't support transactions)
            var uowDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(StockManager.Core.Application.Interfaces.Repositories.IUnitOfWork));
            if (uowDescriptor != null) services.Remove(uowDescriptor);

            services.AddScoped<StockManager.Core.Application.Interfaces.Repositories.IUnitOfWork>(sp =>
            {
                var db = sp.GetRequiredService<StockManagementDbContext>();
                var uowMock = new Mock<StockManager.Core.Application.Interfaces.Repositories.IUnitOfWork>();
                uowMock.Setup(u => u.SaveChangesAsync()).Returns(() => db.SaveChangesAsync());
                uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
                uowMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
                uowMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
                return uowMock.Object;
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StockManagementDbContext>();
            db.Database.EnsureCreated();
        });
        
        builder.UseEnvironment("Testing");
    }
}
