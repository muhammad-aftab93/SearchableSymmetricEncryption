using backend.Entities;
using backend.HelperFunctions.Interfaces;
using backend.Models;
using backend.Repository.Interfaces;
using backend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SseController : ControllerBase
{
    private readonly IRepository _repository;
    private readonly IFileValidator _fileValidator;

    public SseController(IRepository repository,
        IFileValidator fileValidator)
    {
        _repository = repository;
        _fileValidator = fileValidator;
    }

    [HttpPost(Name = "UploadFile")]
    public async Task<Response<object>> UploadFile(IFormFile formFile)
    {
        var response = new Response<object>
        {
            Success = true,
            Message = "Test message"
        };

        try
        {
            // File Validation
            _fileValidator.Validate(formFile);

            // Encryption

            var encryptedFile = new EncryptedFiles()
            {
                FileName = "test filename",
                Content = "test content"
            };

            // Saving file into the database
            // await _repository.SaveEncryptedFileAsync(encryptedFile);

        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    [HttpGet(Name = "SearchFile")]
    public async Task<Response<SearchFileResponse>> SearchFile(string fileName)
    {
        var response = new Response<SearchFileResponse>();

        try
        {

            response.Success = true;
            response.Message = "";
            response.ReturnedObject = new SearchFileResponse()
            {
                FileName = "",
                Url = ""
            };

        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}