using backend.Entities;
using backend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly IRepository _repository;

    public DatabaseController(IRepository repository)
        => _repository = repository;

    [HttpPost(Name = "UploadFile")]
    public async Task<bool> Reset()
    {
        try
        {
            return await _repository.ResetDatabase();
        }
        catch (Exception e)
        {
            return false;
        }
    }
}