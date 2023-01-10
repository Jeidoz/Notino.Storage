using DocumentsStorage.Web.Services.Options;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DocumentsStorage.Web.Services;

public sealed class OutputFormattersService : IOutputFormattersService
    {
        private readonly Dictionary<MediaTypeHeaderValue, OutputFormatter> _outputFormatters;
        public IEnumerable<MediaTypeHeaderValue> SupportedOutputMimeTypes { get; }

        public OutputFormattersService(IOptions<OutputFormattersServiceOptions> options)
        {
            _outputFormatters = options.Value.InitializedOutputFormatters
                .Where(formatter => formatter != null)
                .SelectMany(
                    formatter => formatter.SupportedMediaTypes, 
                    (formatter, mediaType) => new KeyValuePair<MediaTypeHeaderValue, OutputFormatter>(MediaTypeHeaderValue.Parse(mediaType), formatter))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            SupportedOutputMimeTypes = _outputFormatters.Keys;
        }

        public bool HasSupportOf(MediaTypeHeaderValue type)
        {
            return SupportedOutputMimeTypes.Contains(type);
        }

        public bool HasSupportOf(string acceptHeaderValue)
        {
            var hasParsed = MediaTypeHeaderValue.TryParseList(acceptHeaderValue.Split(','), out var types);
            if (!hasParsed)
            {
                throw new ArgumentException($"Application cannot support unknown MIME type: \"{acceptHeaderValue}\"");
            }

            if (types.Count != 1)
            {
                throw new ArgumentException(
                    $"Application does not support multiple types. Please specify a single MIME type for ContentType.");
            }

            return SupportedOutputMimeTypes.Contains(types.First());
        }
    }