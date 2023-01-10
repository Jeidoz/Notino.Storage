using Ardalis.Specification;

namespace DocumentsStorage.Core.Entities.Specifications;

public sealed class DocumentByIdWithTagsAndDataSpec : Specification<Document>, ISingleResultSpecification
{
    public DocumentByIdWithTagsAndDataSpec(string documentId)
    {
        Query.Where(doc => doc.DocumentId == documentId)
            .Include(doc => doc.Tags)
            .Include(doc => doc.Data);
    }
}