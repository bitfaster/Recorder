using Microsoft.AspNetCore.Http;

namespace Recorder
{
    public class DefaultNomenclator : INomenclator
    {
        public string GetName(HttpRequest request)
        {
            return $"{request.Method} {request.Path}{request.QueryString}";
        }
    }
}
