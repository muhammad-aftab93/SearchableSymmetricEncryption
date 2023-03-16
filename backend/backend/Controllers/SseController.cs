using backend.Encryption.Interfaces;
using backend.Entities;
using backend.HelperFunctions.Interfaces;
using backend.Models;
using backend.Repository.Interfaces;
using backend.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using backend.SseOperations.Interfaces;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SseController : ControllerBase
{
    private readonly IRepository _repository;
    private readonly IFileValidator _fileValidator;
    private readonly IEncryption _encryption;
    private readonly Keys _keys;
    private readonly ISseOperations _sseOperations;

    public SseController(
        IRepository repository,
        IFileValidator fileValidator,
        IEncryption encryption,
        IOptions<Keys> keysConfig,
        ISseOperations sseOperations
        )
    {
        _repository = repository;
        _fileValidator = fileValidator;
        _encryption = encryption;
        _keys = keysConfig.Value;
        _sseOperations = sseOperations;
    }

    [HttpPost(Name = "UploadFile")]
    public async Task<Response<object>> UploadFile(IFormFile formFile)
    {
        var response = new Response<object>();

        try
        {
            var keyStr = _keys.Key;
            var hkeyStr = _keys.Hmac;

            // File Validation
            _fileValidator.Validate(formFile);

            string fileName = formFile.FileName;
            using var reader = new StreamReader(formFile.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();

            // Encryption
            var (encryptedContent, encrypedFileName) = _sseOperations.SSEncrypt(
                fileName,
                fileContent,
                Convert.FromBase64String(_keys.Key),
                Convert.FromBase64String(_keys.Hmac)
            );


            var encryptedFile = new EncryptedFiles()
            {
                FileName = Convert.ToBase64String(encrypedFileName),
                Content = Convert.ToBase64String(encryptedContent)
            };

            // Saving file into the database
            int affectedRows = await _repository.SaveEncryptedFileAsync(encryptedFile);
            if (affectedRows > 0)
            {
                response.Success = true;
                response.Message = "File saved successfully.";
            }
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