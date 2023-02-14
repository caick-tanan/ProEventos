using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Domain;
using ProEventos.Application.Contratos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ProEventos.API.Dtos;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {

        private readonly IEventoService _eventoService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EventosController(IEventoService eventoService, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _eventoService = eventoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() // IActionResult serve para retornar em status code ex: 404(Error), 200(Sucesso) ...
        {
            try
            {
                var eventos = await _eventoService.GetAllEventosAsync(true); // retorna todos os eventos com os participantes pois está "true"
                if (eventos == null) return NoContent(); // caso não encontre aparecerá no front o error 404

                return Ok(eventos); // o "OK" serve como se fosse o sucesso utilizando o IActionResult
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(id, true);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{tema}/tema")] // aqui se altera a rota adicionando o /tema para que ele não confunda
        public async Task<IActionResult> GetByTema(string tema)
        {
            try
            {
                var evento = await _eventoService.GetAllEventosByTemaAsync(tema, true);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPost("upload-image/{eventoId}")] // Adicionar Imagem
        public async Task<IActionResult> UploadImage(int eventoId)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(eventoId, true); //meu evento existe ?
                if (evento == null) return NoContent(); // se existir nao vai retornar NoContent

                var file = Request.Form.Files[0];
                if (file.Length > 0) // caso o tamanho seja maior que 0
                {
                    DeleteImage(evento.ImageURL);
                    evento.ImageURL = await SaveImage(file); // altero o nome da imagem recebido do 'evento'
                }

                var EventoRetorno = await _eventoService.UpdateEvento(eventoId, evento); // e aqui eu atualizo o evento com a nova imagem

                return Ok(EventoRetorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar adicionar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPost] // Adicionar
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = await _eventoService.AddEventos(model);
                if (evento == null) return NoContent();
                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar adicionar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPut("{id}")] // Update
        public async Task<IActionResult> Put(int id, EventoDto model)
        {
            try
            {
                var evento = await _eventoService.UpdateEvento(id, model);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar atualizar eventos. Erro: {ex.Message}");
            }
        }

        [HttpDelete("{id}")] // Delete
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var evento = await _eventoService.GetEventoByIdAsync(id, true);
                if (evento == null) return NoContent();

                if (await _eventoService.DeleteEvento(id))
                {
                    DeleteImage(evento.ImageURL);
                    return Ok(new { message = "Deletado" });
                      
                }else
                {
                    throw new Exception("Ocorreu um problem não específico ao tentar deletar Evento.");
                } 
                   
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar deletar eventos. Erro: {ex.Message}");
            }
        }

        [NonAction] //não será um end-point (tipo um post ou um put)
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName) // vai pegar o nome da minha imagem
                                              .Take(10) // Vai pegar os 10 primeiros caracteres da imagem
                                              .ToArray()
                                            ).Replace(' ', '-'); // caso a imagem tenha espaço será colocar um traço '-'

            imageName = $"{imageName}{DateTime.UtcNow.ToString("yymmssfff")}{Path.GetExtension(imageFile.FileName)}"; // vai pegar o nome da imagem passada no File name, adicionar a data e colocar a extensão

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @"Resources/images", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create)) // 
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        [NonAction] //não será um end-point (tipo um post ou um put)
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, @"Resources/Images", imageName); // caminho da minha imagem autal, pegando o diretório
            if (System.IO.File.Exists(imagePath)) // caso o imagePath exista
                System.IO.File.Delete(imagePath); // deleto
        }

    }
}
