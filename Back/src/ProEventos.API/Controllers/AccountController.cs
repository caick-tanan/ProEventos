using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.API.Helpers;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly IUtil _util;
        private readonly string _destino = "perfil";

        public AccountController(IAccountService accountService,
                                 ITokenService tokenService,
                                 IUtil util)
        {
            _util = util;
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userName = User.GetUserName(); // Aqui ele nao está pegando direto do meu Domínio o userName e sim da ClaimsPrincipal, sendo assim minha API não conhece o meu domínio
                var user = await _accountService.GetUserByUserNameAsync(userName);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar recuperar usuário. Erro: {ex.Message}");
            }
        }


        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                if (await _accountService.UserExists(userDto.UserName))
                    return BadRequest("Usuário já existe");

                var user = await _accountService.CreateAccountAsync(userDto);
                if (user != null)
                    return Ok(new 
                    {
                        userName = user.UserName,
                        PrimeiroNome = user.PrimeiroNome,
                        token = _tokenService.CreateToken(user).Result
                    });

                return BadRequest("Usuário não criado, tente novamente mais tarde!");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar Registrar Usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(userLogin.Username); //Usuário existe ?
                if (user == null)
                    return Unauthorized("Usuário ou Senha está errado!");

                var result = await _accountService.CheckUserPasswordAsync(user, userLogin.Password); // Verifica se o usuário e senha estão corretos 
                if(!result.Succeeded) return Unauthorized(); // Se o resultado for diferente de sucesso, Unauthorized(não atuoriza)!
                
                return Ok(new 
                {
                    userName = user.UserName,
                    PrimeiroNome = user.PrimeiroNome,
                    token = _tokenService.CreateToken(user).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar Realizar Login. Erro: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserUpdateDto userUpdateDto)
        {
            try
            {
                if (userUpdateDto.UserName != User.GetUserName())
                    return Unauthorized("Usuário inválido!");

                var user = await _accountService.GetUserByUserNameAsync(User.GetUserName()); //so posso atualizar meu usuário baseado no meu token
                if (user == null) return Unauthorized("Usuário Inválido");

                var userReturn = await _accountService.UpdateAccount(userUpdateDto); 
                if (userReturn == null) return NoContent();

                return Ok(new {
                        userName = userReturn.UserName,
                        PrimeiroNome = userReturn.PrimeiroNome,
                        token = _tokenService.CreateToken(userReturn).Result
                    }
                );
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar Atualizar Usuário. Erro: {ex.Message}");
            }
        }

         [HttpPost("upload-image")] // Adicionar Imagem
        public async Task<IActionResult> UploadImage()
        {
            try
            {
                var user = await _accountService.GetUserByUserNameAsync(User.GetUserName()); //meu evento existe ?
                if (user == null) return NoContent(); // se existir nao vai retornar NoContent

                var file = Request.Form.Files[0];
                if (file.Length > 0) // caso o tamanho seja maior que 0
                {
                    _util.DeleteImage(user.ImageURL, _destino);
                    user.ImageURL = await _util.SaveImage(file, _destino); // altero o nome da imagem recebido do 'evento'
                }

                var EventoRetorno = await _accountService.UpdateAccount(user); // e aqui eu atualizo o evento com a nova imagem

                return Ok(EventoRetorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro ao tentar realizar Upload de Foto do Usuário. Erro: {ex.Message}");
            }
        }
    }
}