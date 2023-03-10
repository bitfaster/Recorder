using Microsoft.Extensions.DependencyInjection;

namespace Recorder
{
    public static class ServiceCollectionExceptions
    {
        public static IServiceCollection AddRequestRecording(this IServiceCollection services)
        {
            return services.AddRequestRecording<DefaultNomenclator>();
        }

        public static IServiceCollection AddRequestRecording<TNomenclator>(this IServiceCollection services) where TNomenclator : class, INomenclator
        {
            services.AddScoped<BlackBox>();
            services.AddSingleton<INomenclator, TNomenclator>();

            return services;
        }
    }
}
