using ProjetoAPI.Data;
using ProjetoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjetoAPI.Repositories
{
    public class EventoRepository
    {
        private readonly ApplicationDbContext _context;

        public EventoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para cadastrar um evento
        public async Task<Evento> Cadastrar(Evento evento)
        {
            try
            {
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
                return evento;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao cadastrar evento", ex);
            }
        }

        // Método para listar todos os eventos
        public async Task<List<Evento>> Listar()
        {
            try
            {
                return await _context.Eventos.Include(e => e.Usuario).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao listar eventos", ex);
            }
        }

        // Método para listar eventos por usuário
        public async Task<List<Evento>> ListarPorUsuario(int usuarioId)
        {
            try
            {
                return await _context.Eventos
                    .Where(e => e.UsuarioId == usuarioId)
                    .Include(e => e.Usuario)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao listar eventos por usuário", ex);
            }
        }
    }
}
