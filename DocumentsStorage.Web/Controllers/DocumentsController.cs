using Ardalis.Result;
using DocumentsStorage.Core.Entities;
using DocumentsStorage.Core.Interfaces;
using DocumentsStorage.Web.ApiModels;
using DocumentsStorage.Web.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DocumentsStorage.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IDocumentsService _documentsService;
        private readonly IOutputFormattersService _formattersService;

        public DocumentsController(ILogger<DocumentsController> logger,
            IDocumentsService documentsService,
            IOutputFormattersService formattersService)
        {
            _logger = logger;
            _documentsService = documentsService;
            _formattersService = formattersService;
        }

        private IActionResult ValidateDocumentSchema(DocumentDTO document)
        {
            if (ModelState.IsValid) return Ok();
            
            var modelStateErrors = ModelState.Keys.SelectMany(key => ModelState[key]?.Errors);
            return BadRequest("Passed document schema contains errors:\n" +
                              string.Join("\n", modelStateErrors.Select(err => err.ErrorMessage).ToArray()));
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return BadRequest("DocumentId cannot be empty.");
            }
            
            try
            {
                var requestedResponseContentType = Request.Headers[HeaderNames.Accept].ToString();
                if (!_formattersService.HasSupportOf(requestedResponseContentType))
                {
                    return BadRequest(
                        $"Our service does not support specified ContentType: \"{requestedResponseContentType}\"." +
                        "Try to verify correctness of ContentType or change it to another MIME type from this list:\n" +
                        string.Join("\n", _formattersService.SupportedOutputMimeTypes));
                }
                
                var document = await _documentsService.FindByDocumentIdAsync(documentId);
                if (!document.IsSuccess)
                {
                    return BadRequest(document.Errors);
                }
                
                return Ok(document.Value.Adapt<DocumentDTO>());
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromBody]DocumentDTO document)
        {
            var state = ValidateDocumentSchema(document);
            if (state is BadRequestResult)
            {
                return state;
            }
            
            try
            {
                var storedDocument = await _documentsService.CreateAsync(document.Adapt<Document>());
                if (!storedDocument.IsSuccess)
                {
                    return BadRequest(storedDocument.Errors);
                }
                return Ok(storedDocument.Value.Adapt<DocumentDTO>());
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument([FromBody]DocumentDTO document)
        {
            var state = ValidateDocumentSchema(document);
            if (state is BadRequestResult)
            {
                return state;
            }
            
            try
            {
                var storedDocument = await _documentsService.UpdateAsync(document.Adapt<Document>());
                if (!storedDocument.IsSuccess)
                {
                    return BadRequest(storedDocument.Errors);
                }
                return Ok(storedDocument.Value.Adapt<DocumentDTO>());
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }
    }
}