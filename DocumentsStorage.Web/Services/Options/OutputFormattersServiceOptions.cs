using Microsoft.AspNetCore.Mvc.Formatters;

namespace DocumentsStorage.Web.Services.Options;

public class OutputFormattersServiceOptions
{
    public IEnumerable<OutputFormatter> InitializedOutputFormatters { get; set; }
}