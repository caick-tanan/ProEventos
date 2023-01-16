using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class PalestrantePersist : IPalestrantePersist
    {
        private readonly ProEventosContext _context;

        public PalestrantePersist(ProEventosContext context)
        {
            _context = context;
        }
       
        public async Task<Palestrante[]> GetAllPalestrantesAsync(bool includeEventos = false)
        {
           IQueryable<Palestrante> query = _context.Palestrantes
                .Include(p => p.RedesSociais); // cria uma query e adicionando as redesSociais desse palestrante

            if (includeEventos) // verifica se quer incluir os eventos
            {
                query = query
                    .Include(p => p.PalestrantesEventos) // caso queria faz a associação com o palestrante evento
                    .ThenInclude(pe => pe.Evento); // aqui irá adicionar o evento do palestrante evento
            }

            query = query.AsNoTracking().OrderBy(p => p.Id); // oredena por ID

            return await query.ToArrayAsync(); // retorna o array 
        }

        public async Task<Palestrante[]> GetAllPalestrantesByNomeAsync(string nome, bool includeEventos)
        {
                       IQueryable<Palestrante> query = _context.Palestrantes
                .Include(p => p.RedesSociais);

            if (includeEventos)
            {
                query = query
                    .Include(p => p.PalestrantesEventos)
                    .ThenInclude(pe => pe.Evento);
            }

            query = query.AsNoTracking().OrderBy(p => p.Id)
                         .Where(p => p.Nome.ToLower().Contains(nome.ToLower()));

            return await query.ToArrayAsync();
        }

        public async Task<Palestrante> GetAllPalestranteByIdAsync(int palestranteId, bool includeEventos)
        {
                       IQueryable<Palestrante> query = _context.Palestrantes
                .Include(p => p.RedesSociais);

            if (includeEventos)
            {
                query = query
                    .Include(p => p.PalestrantesEventos)
                    .ThenInclude(pe => pe.Evento);
            }

            query = query.AsNoTracking().OrderBy(p => p.Id)
                         .Where(p => p.Id == palestranteId);

            return await query.FirstOrDefaultAsync();
        }

    }
}