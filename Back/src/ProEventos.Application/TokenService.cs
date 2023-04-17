using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain.Identity;

namespace ProEventos.Application
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config,
                            UserManager<User> userManager,
                            IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public async Task<string> CreateToken(UserUpdateDto userUpdateDto)
        {
            var user = _mapper.Map<User>(userUpdateDto); // Vai pegar o usuário

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Claim(afirmação) -> id do usuário
                new Claim(ClaimTypes.Name, user.UserName) // Add nome do usuário
            };

            var roles = await _userManager.GetRolesAsync(user); // Vou no banco e busco todas as responsabilidades do usuário

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); // Vai adicionar para dentro de Claim varias outras Claims e adicionando outras roles
        
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor // Criação do Token
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // Data de expiração do token
                SigningCredentials = creds // Chave de criptografia SigningCredentials
            };
             
            var tokenHandler = new JwtSecurityTokenHandler(); // Utilizo esse token para colocar ele em um formato JWT

            var token = tokenHandler.CreateToken(tokenDescription); // Crio meu token adicionando no token

            return tokenHandler.WriteToken(token); // Depois eu escrevo esse token no formato JWT
        }       
    }
}