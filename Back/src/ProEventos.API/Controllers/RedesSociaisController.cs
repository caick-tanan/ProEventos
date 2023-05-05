using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using ProEventos.API.Extensions;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RedesSociaisController : ControllerBase
    {

        private readonly IRedeSocialService _redeSocialService;
        private readonly IEventoService _eventoService;
        private readonly IPalestranteService _palestranteService;

        public RedesSociaisController(IRedeSocialService redeSocialService,
                                      IEventoService eventoService,
                                      IPalestranteService palestranteService)
        {
            _redeSocialService = redeSocialService;
            _eventoService = eventoService;
            _palestranteService = palestranteService;
        }

        [HttpGet("evento/{eventoId}")]
        public async Task<IActionResult> GetByEvento(int eventoId) // IActionResult serve para retornar em status code ex: 404(Error), 200(Sucesso) ...
        {
            try
            {
                if (!(await AutorEvento(eventoId))) // Se não é o autor
                    return Unauthorized(); // Se não é o autor está desautorizado


                var redeSocial = await _redeSocialService.GetAllByEventoIdAsync(eventoId); // retorna todas as rede Sociais caso seja o autor
                if (redeSocial == null) return NoContent(); // caso não encontre aparecerá no front o error 404

                return Ok(redeSocial); // o "OK" serve como se fosse o sucesso utilizando o IActionResult
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar Rede Social por Evento. Erro: {ex.Message}");
            }
        }

        [HttpGet("palestrante")] // O palestrante aqui é o própio usuário logado
        public async Task<IActionResult> GetByPalestrante() // Vou listar todas as redes sociais do palestrante 
        {
            try
            { 
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId()); // listo todos caso ele possua o token
                if (palestrante == null) return Unauthorized();

                var redeSocial = await _redeSocialService.GetAllByPalestranteIdAsync(palestrante.Id); // retorna todos os redeSocial 
                if (redeSocial == null) return NoContent(); // caso não encontre aparecerá no front o error 404

                return Ok(redeSocial); // o "OK" serve como se fosse o sucesso utilizando o IActionResult
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar Rede Social por Palestrante. Erro: {ex.Message}");
            }
        }

        [HttpPut("evento/{eventoId}")] // Update
        public async Task<IActionResult> SaveByEvento(int eventoId, RedeSocialDto[] models)
        {
            try
            {
                if (!(await AutorEvento(eventoId))) // Se não é o autor
                    return Unauthorized(); // Se não é o autor está desautorizado

                var redeSocial = await _redeSocialService.SaveByEvento(eventoId, models); // Caso seja o autor do evento vai savar
                if (redeSocial == null) return NoContent();

                return Ok(redeSocial);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar salvar Rede Social por Evento. Erro: {ex.Message}");
            }
        }

        [HttpPut("palestrante")] // Update
        public async Task<IActionResult> SaveByPalestrante(RedeSocialDto[] models)
        {
            try
            {
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId()); // listo todos caso ele possua o token atual
                if (palestrante == null) return Unauthorized();

                var redeSocial = await _redeSocialService.SaveByPalestrante(palestrante.Id, models); // Caso seja o autor do evento vai savar
                if (redeSocial == null) return NoContent();

                return Ok(redeSocial);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar salvar Rede Social por Palestrante. Erro: {ex.Message}");
            }
        }

        [HttpDelete("evento/{eventoId}/{redeSocialId}")] // Delete
        public async Task<IActionResult> DeleteByEvento(int eventoId, int redeSocialId)
        {
            try
            {                
                if (!(await AutorEvento(eventoId))) // Você é o dono do evento ?
                    return Unauthorized(); // Se não é o autor está desautorizado

                var RedeSocial = await _redeSocialService.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId); // A rede social para esse evento existe?
                if (RedeSocial == null) return NoContent();

                return await _redeSocialService.DeleteByEvento(eventoId, redeSocialId) 
                        ? Ok(new { message = "Rede Social Deletada" }) 
                        : throw new Exception("Ocorreu um problem não específico ao tentar deletar Rede Social por Evento.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar deletar Rede Social por Evento. Erro: {ex.Message}");
            }
        }

        [HttpDelete("palestrante/{redeSocialId}")] // Delete
        public async Task<IActionResult> DeleteByPalestrante(int redeSocialId)
        {
            try
            {                
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId()); // listo todos caso ele possua o token atual
                if (palestrante == null) return Unauthorized();

                var RedeSocial = await _redeSocialService.GetRedeSocialPalestranteByIdsAsync(palestrante.Id, redeSocialId); // Uso o palestrante.Id pois eu recupero baseado no usuário logado já
                if (RedeSocial == null) return NoContent();

                return await _redeSocialService.DeleteByPalestrante(palestrante.Id, redeSocialId) 
                        ? Ok(new { message = "Rede Social Deletada" }) 
                        : throw new Exception("Ocorreu um problem não específico ao tentar deletar Rede Social por Palestrante.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar deletar Rede Social por Palestrante. Erro: {ex.Message}");
            }
        }



        [NonAction]
        private async Task<bool> AutorEvento(int eventoId) // Essa função serve para saber de quem é o token que está logado fazendo as alterações é o autor do evento
        {
            var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, false); // caso nao liste nenhum evento, significa que o esse evento não é do usuário que está logado
            if (evento == null) return false; // Ou seja ele nao é o autor

            return true; // Se for retorna True
        }
    }
}
