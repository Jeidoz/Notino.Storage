namespace DocumentsStorage.Core.Entities;

public sealed class DataPayload : Entity
{
    public string ProjectName { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
}