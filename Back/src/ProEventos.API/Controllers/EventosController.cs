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
using ProEventos.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using ProEventos.Persistence.Models;
using ProEventos.API.Helpers;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {

        private readonly IEventoService _eventoService;
        private readonly IUtil _util;
        private readonly IAccountService _accountService;

        private readonly string _destino = "Images";

        public EventosController(IEventoService eventoService, IUtil util, IAccountService accountService)
        {
            _util = util;
            _accountService = accountService;
            _eventoService = eventoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PageParams pageParams) // IActionResult serve para retornar em status code ex: 404(Error), 200(Sucesso) ...
        {
            try
            {
                var eventos = await _eventoService.GetAllEventosAsync(User.GetUserId(), pageParams, true); // retorna todos os eventos com os participantes pois está "true"
                if (eventos == null) return NoContent(); // caso não encontre aparecerá no front o error 404

                Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);

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
                var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), id, true);
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
                var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, true); //meu evento existe ?
                if (evento == null) return NoContent(); // se existir nao vai retornar NoContent

                var file = Request.Form.Files[0];
                if (file.Length > 0) // caso o tamanho seja maior que 0
                {
                    _util.DeleteImage(evento.ImageURL, _destino);
                    evento.ImageURL = await _util.SaveImage(file, _destino); // altero o nome da imagem recebido do 'evento'
                }

                var EventoRetorno = await _eventoService.UpdateEvento(User.GetUserId(), eventoId, evento); // e aqui eu atualizo o evento com a nova imagem

                return Ok(EventoRetorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar realizar Upload de Foto do Usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost] // Adicionar
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = await _eventoService.AddEventos(User.GetUserId(), model);
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
                var evento = await _eventoService.UpdateEvento(User.GetUserId(), id, model);
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
                var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), id, true);
                if (evento == null) return NoContent();

                if (await _eventoService.DeleteEvento(User.GetUserId(), id))
                {
                    _util.DeleteImage(evento.ImageURL, _destino);
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
    }
}