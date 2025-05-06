using Microsoft.AspNetCore.Mvc;
using ProjetoAPI.Models;
using ProjetoAPI.Repositories;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            UsuarioRepository usuarioRepository, 
            IConfiguration configuration,
            ILogger<UsuariosController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            try
            {
                var usuarioExistente = await _usuarioRepository.BuscarPorEmail(usuario.Email);
                if (usuarioExistente != null)
                    return BadRequest(new { mensagem = "Email já cadastrado" });

                // ⚠️ Exemplo apenas - substitua por hashing seguro (ex: BCrypt)
                usuario.Senha = HashPassword(usuario.Senha);
                
                var novoUsuario = await _usuarioRepository.Cadastrar(usuario);
                return CreatedAtAction(nameof(Cadastrar), new { id = novoUsuario.Id }, novoUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuário");
                return StatusCode(500, new { mensagem = "Erro interno no servidor" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            try
            {
                var usuarioBanco = await _usuarioRepository.BuscarPorEmail(usuario.Email);
                if (usuarioBanco == null || !VerifyPassword(usuario.Senha, usuarioBanco.Senha))
                    return Unauthorized(new { mensagem = "Email ou senha inválidos" });

                var token = GenerateJwtToken(usuarioBanco.Email, usuarioBanco.Id.ToString());
                
                return Ok(new 
                {
                    token,
                    expiraEm = DateTime.Now.AddHours(1),
                    usuarioId = usuarioBanco.Id,
                    email = usuarioBanco.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no login");
                return StatusCode(500, new { mensagem = "Erro interno no servidor" });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var usuarios = await _usuarioRepository.Listar();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários");
                return StatusCode(500, new { mensagem = "Erro interno no servidor" });
            }
        }

        private string GenerateJwtToken(string email, string userId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, email) // <- Esse é o segredo para funcionar com User.Identity.Name
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Chave JWT não configurada")));
                
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            // Apenas exemplo - substitua por uma hash segura em produção
            return password;
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            // Apenas exemplo - substitua por verificação segura
            return inputPassword == hashedPassword;
        }
    }
}
