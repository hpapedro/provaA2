using Microsoft.AspNetCore.Mvc;
using ProjetoAPI.Models;
using ProjetoAPI.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoAPI.Controllers
{
    [Route("eventos")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly EventoRepository _eventoRepository;
        private readonly UsuarioRepository _usuarioRepository;

        public EventosController(EventoRepository eventoRepository, UsuarioRepository usuarioRepository)
        {
            _eventoRepository = eventoRepository;
            _usuarioRepository = usuarioRepository;
        }

        // Endpoint para listar todos os eventos
        [HttpGet("listar")]
        public async Task<ActionResult<List<Evento>>> Listar()
        {
            try
            {
                var eventos = await _eventoRepository.Listar();
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao listar eventos", erro = ex.Message });
            }
        }

        // Endpoint para listar eventos de um usuário específico
        [Authorize]
        [HttpGet("usuario")]
        public async Task<ActionResult<List<Evento>>> ListarPorUsuario()
        {
            var email = User.Identity?.Name;
            if (email == null)
                return Unauthorized();

            var usuario = await _usuarioRepository.BuscarPorEmail(email);
            if (usuario == null)
                return Unauthorized();

            var eventos = await _eventoRepository.ListarPorUsuario(usuario.Id);
            return Ok(eventos);
        }

        // Endpoint para cadastrar um evento
        [Authorize]
        [HttpPost("cadastrar")]
        public async Task<ActionResult<Evento>> Cadastrar([FromBody] Evento evento)
        {
            try
            {
                var email = User.Identity?.Name;
                if (email == null)
                    return Unauthorized();

                var usuario = await _usuarioRepository.BuscarPorEmail(email);
                if (usuario == null)
                    return Unauthorized();

                evento.UsuarioId = usuario.Id;
                var novoEvento = await _eventoRepository.Cadastrar(evento);
                return Ok(novoEvento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao cadastrar evento", erro = ex.Message });
            }
        }
    }
}

