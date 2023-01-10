using Microsoft.Net.Http.Headers;

namespace DocumentsStorage.Web.Services;

public interface IOutputFormattersService
{
    IEnumerable<MediaTypeHeaderValue> SupportedOutputMimeTypes { get; }
        
    bool HasSupportOf(MediaTypeHeaderValue type);
    bool HasSupportOf(string acceptHeaderValue);
}