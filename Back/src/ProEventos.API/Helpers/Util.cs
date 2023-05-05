using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Helpers;

namespace ProEventos.API.Helpers
{
    public class Util : IUtil
    {

        private readonly IWebHostEnvironment _hostEnvironment;

        public Util(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<string> SaveImage(IFormFile imageFile, string destino)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName) // vai pegar o nome da minha imagem
                                              .Take(10) // Vai pegar os 10 primeiros caracteres da imagem
                                              .ToArray()
                                            ).Replace(' ', '-'); // caso a imagem tenha espaço será colocar um traço '-'

            imageName = $"{imageName}{DateTime.UtcNow.ToString("yymmssfff")}{Path.GetExtension(imageFile.FileName)}"; // vai pegar o nome da imagem passada no File name, adicionar a data e colocar a extensão

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @$"Resources/{destino}", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create)) 
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        public void DeleteImage(string imageName, string destino)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @$"Resources/{destino}", imageName); // caminho da minha imagem autal, pegando o diretório
            if (System.IO.File.Exists(imagePath)) // caso o imagePath exista
                System.IO.File.Delete(imagePath); // deleto
        }
       
    }
}