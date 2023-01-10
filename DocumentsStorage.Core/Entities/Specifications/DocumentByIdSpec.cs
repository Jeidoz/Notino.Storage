using Ardalis.Specification;

namespace DocumentsStorage.Core.Entities.Specifications;

public sealed class DocumentByIdSpec : Specification<Document>, ISingleResultSpecification
{
    public DocumentByIdSpec(string documentId)
    {
        Query.Where(doc => doc.DocumentId == documentId);
    }
}