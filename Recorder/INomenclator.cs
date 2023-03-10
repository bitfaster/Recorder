using Microsoft.AspNetCore.Http;

namespace Recorder
{
    public interface INomenclator
    {
        string GetName(HttpRequest request);
    }
}
