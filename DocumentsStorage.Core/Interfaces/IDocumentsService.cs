using Ardalis.Result;
using DocumentsStorage.Core.Entities;

namespace DocumentsStorage.Core.Interfaces;

public interface IDocumentsService
{
    Task<Result<Document>> CreateAsync(Document newDocument);
    Task<Result<Document>> FindByDocumentIdAsync(string documentId);
    Task<Result<Document>> UpdateAsync(Document documentToUpdate);
}