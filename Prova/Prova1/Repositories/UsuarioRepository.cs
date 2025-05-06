using ProjetoAPI.Data;
using ProjetoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjetoAPI.Repositories
{
    public class UsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para cadastrar usuário
        public async Task<Usuario> Cadastrar(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao cadastrar usuário", ex);
            }
        }

        // Método para buscar usuário por email
        public async Task<Usuario> BuscarPorEmail(string email)
        {
            try
            {
                return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar usuário por email", ex);
            }
        }

        // Método para listar usuários
        public async Task<List<Usuario>> Listar()
        {
            try
            {
                return await _context.Usuarios.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao listar usuários", ex);
            }
        }
    }
}
