using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ProEventos.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using ProEventos.Persistence.Models;
using ProEventos.Application.Dtos;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PalestrantesController : ControllerBase
    {

        private readonly IPalestranteService _palestranteService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAccountService _accountService;

        public PalestrantesController(IPalestranteService palestranteService, IWebHostEnvironment hostEnvironment, IAccountService accountService)
        {
            _hostEnvironment = hostEnvironment;
            _accountService = accountService;
            _palestranteService = palestranteService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery]PageParams pageParams) // IActionResult serve para retornar em status code ex: 404(Error), 200(Sucesso) ...
        {
            try
            {
                var palestrantes = await _palestranteService.GetAllPalestrantesAsync(pageParams, true); // retorna todos os palestrantes com os participantes pois está "true"
                if (palestrantes == null) return NoContent(); // caso não encontre aparecerá no front o error 404

                Response.AddPagination(palestrantes.CurrentPage,
                                    palestrantes.PageSize,
                                    palestrantes.TotalCount,
                                    palestrantes.TotalPages);

                return Ok(palestrantes); // o "OK" serve como se fosse o sucesso utilizando o IActionResult
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar palestrantes. Erro: {ex.Message}");
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetPalestrantes()
        {
            try
            {
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId(), true);
                if (palestrante == null) return NoContent();

                return Ok(palestrante);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar palestrantes. Erro: {ex.Message}");
            }
        }


        [HttpPost] // Adicionar
        public async Task<IActionResult> Post(PalestranteAddDto model)
        {
            try
            {
                var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId(), false); // Verifica se o meu usuário já possui um palestrante
                if (palestrante == null) // Caso meu palestrante não exista
                    palestrante = await _palestranteService.AddPalestrantes(User.GetUserId(), model); // Crio ele aqui
                
                return Ok(palestrante); // E retorno ele aqui
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar adicionar palestrantes. Erro: {ex.Message}");
            }
        }

        [HttpPut] // Update
        public async Task<IActionResult> Put(PalestranteUpdateDto model)
        {
            try
            {
                var palestrante = await _palestranteService.UpdatePalestrante(User.GetUserId(), model);
                if (palestrante == null) return NoContent();

                return Ok(palestrante);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar atualizar palestrantes. Erro: {ex.Message}");
            }
        }
    }
}