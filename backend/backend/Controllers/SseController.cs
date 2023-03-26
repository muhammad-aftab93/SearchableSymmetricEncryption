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
using Microsoft.AspNetCore.StaticFiles;

namespace backend.Controllers;

[ApiController]
[Route("[controller]/[action]")]
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
            // File Validation
            _fileValidator.Validate(formFile);

            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            using var reader = new StreamReader(formFile.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();

            // Encryption
            var (encryptedContent, encrypedFileName) = _sseOperations.SsEncrypt(
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

    [HttpPost(Name = "SearchFile")]
    public async Task<Response<SearchFileResponse>> SearchFile([FromForm] string fileName)
    {
        var response = new Response<SearchFileResponse>();

        try
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            var (fileName1, fileContent) = await _sseOperations.SsSearch(
                        fileName,
                        Convert.FromBase64String(_keys.Key),
                        Convert.FromBase64String(_keys.Hmac)
                    );

            if (string.IsNullOrEmpty(fileName1))
            {
                response.Success = false;
                response.Message = "File not found!";
            }
            else
            {
                // Decrypt found file
                var decryptedContent = _sseOperations.SsDecrypt(
                    Convert.FromBase64String(fileContent ?? ""),
                    Convert.FromBase64String(_keys.Key));

                var directory = "TempFiles";
                var filename = $"{fileName}.txt";

                if(!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await using var streamWriter = new StreamWriter($"{directory}/{filename}", false);
                await streamWriter.WriteAsync(decryptedContent);

                response.Success = true;
                response.Message = "File found.";
                response.ReturnedObject = new SearchFileResponse()
                {
                    FileName = $"{fileName}.txt",
                    FileContent = decryptedContent,
                    Url = ""
                };
            }
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    [HttpGet]
    [Route("{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "TempFiles", fileName);

        var memory = new MemoryStream();
        await using var stream = new FileStream(path, FileMode.Open);
        await stream.CopyToAsync(memory);
        memory.Position = 0;

        return File(memory, "application/octet-stream", Path.GetFileName(path));
    }
}