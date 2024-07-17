using Microsoft.AspNetCore.Mvc;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Net.Http.Headers;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var streamContent = new StreamContent(file.OpenReadStream());
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = "file",
                                FileName = file.FileName
                            };
                            formData.Add(streamContent, "files", file.FileName);
                        }
                    }

                    try        
                    {
                        var response = await client.PostAsync("https://localhost:7000/api/PdfProcess", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            // Handle success
                            return Ok();
                        }
                        else
                        {
                            // Handle failure
                            return StatusCode((int)response.StatusCode);
                        }
                    } 
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
