using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.Dtos;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotesController : ControllerBase
    {

        private readonly ILoteService _loteService;

        public LotesController(ILoteService LoteService)
        {
            _loteService = LoteService;
        }

        [HttpGet("{eventoId}")]
        public async Task<IActionResult> Get(int eventoId) // IActionResult serve para retornar em status code ex: 404(Error), 200(Sucesso) ...
        {
            try
            {
                var lotes = await _loteService.GetLotesByEventoIdAsync(eventoId); // retorna todos os lotes 
                if (lotes == null) return NoContent(); // caso não encontre aparecerá no front o error 404

                return Ok(lotes); // o "OK" serve como se fosse o sucesso utilizando o IActionResult
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar lotes. Erro: {ex.Message}");
            }
        }

        [HttpPut("{eventoId}")] // Update
        public async Task<IActionResult> SaveLotes(int eventoId, LoteDto[] models)
        {
            try
            {
                var lotes = await _loteService.SaveLotes(eventoId, models);
                if (lotes == null) return NoContent();

                return Ok(lotes);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar salvar lotes. Erro: {ex.Message}");
            }
        }

        [HttpDelete("{eventoId}/{loteId}")] // Delete
        public async Task<IActionResult> Delete(int eventoId, int loteId)
        {
            try
            {
                var lote = await _loteService.GetLoteByIdsAsync(eventoId, loteId);
                if (lote == null) return NoContent();

                 return await _loteService.DeleteLote(lote.EventoId, lote.Id) 
                       ? Ok(new { message = "Lote Deletado" }) 
                       : throw new Exception("Ocorreu um problem não específico ao tentar deletar Lote.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar deletar lotes. Erro: {ex.Message}");
            }
        }
    }
}
