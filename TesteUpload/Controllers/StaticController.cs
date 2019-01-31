using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        UploadFilesConfiguration UploadConfig { get; }
        public StaticController(UploadFilesConfiguration uploadConfig)
        {
            UploadConfig = uploadConfig;
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post([FromForm(Name = "file")] IFormFile file)
        {
            if (file == null)
            {
                return NotFound("Arquivo não enviado");
            }

            var filePath = string.Empty;

            FileValidation(file);

            filePath = string.Format(GetLocalPath(file), GetLocalFileName(file));

            using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileReturn = UploadConfig.StaticCurrentDomain + filePath;

            return Ok(fileReturn);

        }

        private void FileValidation(IFormFile formFile)
        {
            var formatos = UploadConfig.Formats.Split(',');
            if (!formatos.Any(f => f.Trim().ToLower().Equals(formFile.ContentType)))
            {
                throw new UnsupportedMediaTypeException("Tipo de arquivo não suportado", new MediaTypeHeaderValue(formFile.ContentType));
            }
            else if (UploadConfig.MaxLength <= formFile.Length)
            {
                throw new UnsupportedMediaTypeException("Tamanho de arquivo não suportado", new MediaTypeHeaderValue(formFile.ContentType));
            }
        }

        private string GetLocalPath(IFormFile ff)
        {
            var format = ff.ContentType;
            if (format.Equals("image/png") || format.Equals("image/jpeg") || format.Equals("image/jpg"))
            {
                return UploadConfig.ImageFolder;
            }
            else
            {
                return UploadConfig.DocumentFolder;
            }
        }

        private string GetLocalFileName(IFormFile ff)
        {
            try
            {
                string fileName = !string.IsNullOrWhiteSpace(ff.FileName) ? ff.FileName : "SemNome.data";

                var format = ff.ContentType;
                if (format.Equals("image/png") || format.Equals("image/jpeg") || format.Equals("image/jpg"))
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

    public class UploadFilesConfiguration
    {
        public string Formats { get; set; }

        public int MaxLength { get; set; }

        public string ImageFolder { get; set; }

        public string  DocumentFolder { get; set; }

        public string StaticCurrentDomain { get; set; }
    }

  
}