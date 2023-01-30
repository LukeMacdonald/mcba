using A2Practice.Controllers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CustomerPortal.Controllers;
using CustomerPortal.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomerPortalUnitTests.Modules;

// THIS CODE CAME FROM LECTURES GIVEN
public class BackendModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var services = new ServiceCollection();

        services.AddControllersWithViews().AddApplicationPart(typeof(AccountController).Assembly)
            .AddControllersAsServices();
        services.AddControllersWithViews().AddApplicationPart(typeof(CustomerController).Assembly)
            .AddControllersAsServices();
        services.AddControllersWithViews().AddApplicationPart(typeof(BillPayController).Assembly)
            .AddControllersAsServices();
        services.AddControllersWithViews().AddApplicationPart(typeof(LoginController).Assembly)
            .AddControllersAsServices();

        services.AddDbContext<MCBAContext>(options =>
        {
            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            options.UseLazyLoadingProxies();
        });
        
        builder.Populate(services);
        

        builder.Register(_ =>
        {
            var cacheOptions = new MemoryDistributedCacheOptions();
            var options = Options.Create(cacheOptions);
            var memoryDistributedCache = new MemoryDistributedCache(options);
            var factory = LoggerFactory.Create(b => b.AddConsole());
            var distributedSession = new DistributedSession(memoryDistributedCache, Guid.NewGuid().ToString(),
                TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(1), () => true, factory, true);
            return (ISession)distributedSession;
        });

        builder.RegisterType<DefaultHttpContext>().As<HttpContext>();
        
        // builder.Register(_ =>
        // {
        //     var context = new MCBAContext(
        //         new DbContextOptionsBuilder<MCBAContext>().UseInMemoryDatabase())
        // })
        
    }
}