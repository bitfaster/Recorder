using Microsoft.AspNetCore.Builder;

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
