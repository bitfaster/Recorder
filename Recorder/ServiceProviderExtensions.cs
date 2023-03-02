using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
