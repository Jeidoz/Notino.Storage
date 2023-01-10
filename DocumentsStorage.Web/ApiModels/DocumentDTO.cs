using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using DocumentsStorage.Core.Entities;
using MessagePack;

namespace DocumentsStorage.Web.ApiModels;

[MessagePackObject(keyAsPropertyName: true)]
public class DocumentDTO
{
    [Required(ErrorMessage = "DocumentId cannot be empty.", AllowEmptyStrings = false)]
    [DataMember]
    public string Id { get; set; }
        
    [Required(ErrorMessage = "Tags field is required."), 
     MinLength(1, ErrorMessage = "Document must have at least 1 tag.")]
    [DataMember]
    public string[] Tags { get; set; }
        
    [Required(ErrorMessage = "Data field is required.")]
    [DataMember]
    public DataPayloadDTO Data { get; set; }
}

public class DataPayloadDTO
{
    [DataMember]
    public string Author { get; set; }
    
    [Required(ErrorMessage = "Project name cannot be empty.", AllowEmptyStrings = false)]
    [DataMember]
    public string ProjectName { get; set; }
    
    [DataMember]
    public string Description { get; set; }
}

