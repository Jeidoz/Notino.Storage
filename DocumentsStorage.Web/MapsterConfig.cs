
using System.Reflection;
using DocumentsStorage.Core.Entities;
using DocumentsStorage.Web.ApiModels;
using Mapster;

namespace DocumentsStorage.Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<Document, DocumentDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.DocumentId)
            .Map(dest => dest.Tags, src => src.Tags.Select(tag => tag.Name).ToArray());

        TypeAdapterConfig<DocumentDTO, Document>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Map(dest => dest.Tags, src => src.Tags.Select(t => new Tag { Name = t }).ToArray())
            .Map(dest => dest.DocumentId, src => src.Id);
    }
}