using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.Middleware
{
    public static class RecorderMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestRecorder(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<RecorderMiddleware>();
        }
    }
}
