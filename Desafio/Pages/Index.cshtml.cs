using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using MediaToolkit;
using MediaToolkit.Model;
using VideoLibrary;

namespace Desafio.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;


        public IndexModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public IFormFile VideoFile { get; set; }

        public IActionResult OnPost()
        {
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var tempVideoPath = Path.Combine(_webHostEnvironment.WebRootPath, "videos", VideoFile.FileName);

                using (var fileStream = new FileStream(tempVideoPath, FileMode.Create))
                {
                    VideoFile.CopyTo(fileStream);
                }

                var inputFile = new MediaFile { Filename = tempVideoPath };
                var engine = new Engine();
                engine.GetMetadata(inputFile);

                var videoInfo = new
                {
                    FileName = inputFile.Filename,
                    ContentType = VideoFile.ContentType,
                    Size = VideoFile.Length,
                    Duration = inputFile.Metadata.Duration,

                };

                return new JsonResult(videoInfo);
            }

            return BadRequest("Arquivo não encontrado");
        }

        private byte[] GetFileBytes(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                return stream.ToArray();
            }
        }
    }
}