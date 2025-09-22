using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Bootstrapper;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FullStackAssignment.IntegrationTests
{
    public class WebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _tempFolder;

        public WebAppFactory()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "FullStackAssignmentTestImages");
            Directory.CreateDirectory(_tempFolder);

            var staticFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            if (!Directory.Exists(staticFolder))
                Directory.CreateDirectory(staticFolder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ProductImagesFolder", _tempFolder }
                });
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("DatabaseForTesting");
                });

                var fileRepoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFileRepo));
                if (fileRepoDescriptor != null)
                    services.Remove(fileRepoDescriptor);

                services.AddScoped<IFileRepo>(_ => new TestFileRepo(_tempFolder));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        }
    }
    
}