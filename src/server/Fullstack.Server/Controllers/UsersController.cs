using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Fullstack.Server.Data;
using Fullstack.Server.Models;

namespace Fullstack.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public UsersController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return await _db.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User updated)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Name = updated.Name;
        user.Email = updated.Email;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(user.Avatar))
        {
            var filePath = Path.Combine(_env.ContentRootPath, user.Avatar.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/avatar")]
    public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded" });

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{id}_{Guid.NewGuid()}{ext}";
        var relativePath = Path.Combine("uploads", "avatars", fileName);
        var absolutePath = Path.Combine(_env.ContentRootPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        if (!string.IsNullOrEmpty(user.Avatar))
        {
            var oldPath = Path.Combine(_env.ContentRootPath, user.Avatar.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        using (var stream = new FileStream(absolutePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.Avatar = "/" + relativePath.Replace("\\", "/");
        await _db.SaveChangesAsync();

        return Ok(new { avatarUrl = user.Avatar });
    }

    [HttpDelete("{id}/avatar")]
    public async Task<IActionResult> DeleteAvatar(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(user.Avatar))
        {
            var filePath = Path.Combine(_env.ContentRootPath, user.Avatar.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            user.Avatar = null;
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }
}
