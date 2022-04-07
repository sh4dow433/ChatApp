using ChatApi.Models;
using ChatApi.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesManager _filesManager;
        public FilesController(IFilesManager filesManager)
        {
            _filesManager = filesManager;
        }
        public IActionResult UploadFile (IFormFile file, [FromQuery]bool isPhoto = false)
        {
            var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var fileModel = new FileModel()
            {
                UserId = id.Value,
                IsPhoto = isPhoto,
                //UserId = "13efb77f-1563-4d6d-9463-e85f29548689",
                File = file
            };
            var fileRecord = _filesManager.SaveFile(fileModel);
            if (fileRecord == null)
            {
                return BadRequest();
            }
            return Ok(fileRecord);
        }
    }
}
