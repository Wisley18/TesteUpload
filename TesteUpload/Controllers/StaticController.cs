using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using S9.Web.Helpers.FileManager;

namespace TesteUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticController : ControllerBase
    {

        public string ImageFolder { get; set; }

        public string DocumentFolder { get; set; }


        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post([FromForm(Name = "file")] List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePath = "Content/uploads/static/_docs/{0}";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    //if (formFile.ContentType.Any(f => f.Equals()))
                    //{

                    //}
                    using (var stream = new FileStream(string.Format(filePath, GetLocalFileName(formFile)), FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }

        private void FileValidation(List<IFormFile> colFormFile)
        {
            var isNotValid = colFormFile.Any(f => 
                                                f.ContentType != "image/png" || 
                                                f.ContentType != "image/jpeg" || 
                                                f.ContentType != "image/jpg" ||
                                                f.ContentType != "application/pdf" ||
                                                f.ContentType != "text/plain");
            if (true)
            {

            }
        }

        private string GetLocalFileName(IFormFile ff)
        {
            try
            {
                string fileName = !string.IsNullOrWhiteSpace(ff.FileName) ? ff.FileName : "SemNome.data";

                var format = ff.ContentType;
                if ((format.Equals("image/png") || format.Equals("image/jpeg") || format.Equals("image/jpg") || format.Equals("video/mp4")))
                {
                    fileName = "." + format.Split('/')[1];
                }
                else
                {
                    fileName = "__" + fileName;
                }

                fileName = (Guid.NewGuid().ToString() + fileName).Replace("-", "_");
                fileName = fileName.Replace("\"", string.Empty); //isto é colocado aqui porque o Chrome envia arquivos entre aspas, que são tratados como parte do nome do arquivo.

                return fileName;
            }
            catch
            {
                return Guid.NewGuid().ToString();
            }

        }
    }

  
}