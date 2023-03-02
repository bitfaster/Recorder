using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.Middleware
{
    // https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/HealthChecks/src/Builder/HealthCheckApplicationBuilderExtensions.cs
    // https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/HealthChecks/src/Builder/HealthCheckEndpointRouteBuilderExtensions.cs
    public class RecorderMiddleware
    {
        private readonly RequestDelegate _next;

        public RecorderMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context, BlackBox blackBox)
        {
            blackBox.Name = $"{context.Request.Method} {context.Request.Path}";

            await _next(context);

            blackBox.End();
        }
    }
}
