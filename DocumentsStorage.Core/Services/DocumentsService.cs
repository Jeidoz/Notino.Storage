using Ardalis.Result;
using Ardalis.Specification;
using DocumentsStorage.Core.Entities;
using DocumentsStorage.Core.Entities.Specifications;
using DocumentsStorage.Core.Interfaces;

namespace DocumentsStorage.Core.Services;

public sealed class DocumentsService : IDocumentsService
{
    private readonly IRepositoryBase<Document> _repository;

    public DocumentsService(IRepositoryBase<Document> repository)
    {
        _repository = repository;
    }

    public async Task<Result<Document>> CreateAsync(Document newDocument)
    {
        string documentId = newDocument.DocumentId;
        var existingDocument = await FindByDocumentIdAsync(documentId);
        if (existingDocument.IsSuccess) 
        {
            var errors = new List<ValidationError>
            {
                new() { Identifier = nameof(newDocument.DocumentId), ErrorMessage = $"Document with ID '{documentId}' already exists."}
            };
            return Result<Document>.Invalid(errors);
        }
        
        var storedDocument = await _repository.AddAsync(newDocument);
        await _repository.SaveChangesAsync();
        return new Result<Document>(storedDocument);
    }

    public async Task<Result<Document>> FindByDocumentIdAsync(string documentId)
    {
        var documentSpec = new DocumentByIdWithTagsAndDataSpec(documentId);
        var existingDocument = await _repository.FirstOrDefaultAsync(documentSpec);
        return existingDocument is null 
            ? Result<Document>.NotFound($"Document with specified id: '{documentId}' does not exist.") 
            : new Result<Document>(existingDocument);
    }

    public async Task<Result<Document>> UpdateAsync(Document documentToUpdate)
    {
        string documentId = documentToUpdate.DocumentId;
        var documentSearchSpec = new DocumentByIdWithTagsAndDataSpec(documentId);
        var existingDocument = await _repository.FirstOrDefaultAsync(documentSearchSpec);
        if (existingDocument is null)
        {
            var errors = new List<ValidationError>
            {
                new() { Identifier = nameof(documentToUpdate.DocumentId), ErrorMessage = $"Document with id '{documentId}' does not exist."}
            };
            return Result<Document>.Invalid(errors);
        }
        
        existingDocument.UpdateData(documentToUpdate.Data);
        existingDocument.UpdateTags(documentToUpdate.Tags);
        await _repository.UpdateAsync(existingDocument);
        await _repository.SaveChangesAsync();
        return new Result<Document>(existingDocument);
    }
}