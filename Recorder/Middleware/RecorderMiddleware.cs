using Microsoft.AspNetCore.Http;

namespace Recorder.Middleware
{
    public class RecorderMiddleware
    {
        private readonly RequestDelegate _next;

        public RecorderMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context, BlackBox blackBox, INomenclator nomenclator)
        {
            blackBox.Name = nomenclator.GetName(context.Request);

            await _next(context);

            blackBox.End();
        }
    }
}
