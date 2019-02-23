using AzureTableEventSourcingTest.Infrastructure;
using AzureTableEventSourcingTest.WebApi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddInitializable<TService>(this IServiceCollection services)
            where TService : IInitializable
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IBeforeApplicationStart, BeforeApplicationStartInitializableAdapter>(sp
                => new BeforeApplicationStartInitializableAdapter(sp.GetRequiredService<TService>())));
            return services;
        }
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    public static class Extensions
    {
        public static async Task BeforeApplicationStart(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices<IBeforeApplicationStart>();
                await Task.WhenAll(handlers.Select(x => x.OnBeforeApplicationStartAsync()));
            }
        }
    }
}
