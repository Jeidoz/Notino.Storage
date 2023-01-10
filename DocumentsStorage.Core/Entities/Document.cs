namespace DocumentsStorage.Core.Entities;

public class Document : Entity
{
    public string DocumentId { get; set; }
    public virtual ICollection<Tag> Tags { get; set; }
    public DataPayload Data { get; set; }

    public void UpdateData(DataPayload data)
    {
        Data = data ?? throw new NullReferenceException("Data field cannot be null.");
    }

    public void UpdateTags(ICollection<Tag> tags)
    {
        Tags = tags ?? throw new NullReferenceException("Tags cannot be null.");
    }
}