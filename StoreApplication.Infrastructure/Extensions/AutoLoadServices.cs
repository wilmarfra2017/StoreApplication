using Microsoft.Extensions.DependencyInjection;
using StoreApplication.Domain.Ports;
using StoreApplication.Domain.Services;
using StoreApplication.Infrastructure.Adapters;
using StoreApplication.Infrastructure.Ports;

namespace StoreApplication.Infrastructure.Extensions;

public static class AutoLoadServices
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ILogMessageService), typeof(LogMessageImple));
        services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddTransient(typeof(IProductAvailabilityService), typeof(ProductAvailabilityService));
        services.AddTransient<ICredentialsRepository, CredentialsRepository>();
        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IProductRepository, ProductRepository>();

        var _services = AppDomain.CurrentDomain.GetAssemblies()
                  .Where(assembly =>
                  {
                      return (assembly.FullName is null) || assembly.FullName.Contains("Domain", StringComparison.InvariantCulture);
                  })
                  .SelectMany(s => s.GetTypes())
                  .Where(p => p.CustomAttributes.Any(x => x.AttributeType == typeof(DomainServiceAttribute)));


        var _repositories = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly =>
            {
                return (assembly.FullName is null) || assembly.FullName.Contains("Infrastructure", StringComparison.InvariantCulture);
            })
            .SelectMany(s => s.GetTypes())
            .Where(p => p.CustomAttributes.Any(x => x.AttributeType == typeof(RepositoryAttribute)));


        foreach (var service in _services)
        {
            services.AddTransient(service);
        }


        foreach (var repo in _repositories)
        {
            Type? iface = repo.GetInterfaces().SingleOrDefault();
            if (iface == null)
            {
                throw new InvalidOperationException($"The type {repo.Name} does not implement any interface or implements more than one.");
            }
            services.AddTransient(iface, repo);

        }

        return services;
    }
 }
