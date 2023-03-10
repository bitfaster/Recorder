using Microsoft.Extensions.DependencyInjection;

namespace Recorder
{
    public static class ServiceProviderExtensions
    {
        public static IDisposable RecordStackFrame(this IServiceProvider sp, string stackFrameName)
        {
            var blackBox = sp.GetService<BlackBox>();
            return blackBox.Capture(stackFrameName);
        }
    }
}
