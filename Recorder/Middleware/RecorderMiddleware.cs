using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.Middleware
{
    public class RecorderMiddleware
    {
        private readonly RequestDelegate _next;

        public RecorderMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context, BlackBox blackBox)
        {
            await _next(context);

            blackBox.End();
        }
    }
}
