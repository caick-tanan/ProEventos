using System.Threading.Tasks;
using ProEventos.Application.Dtos;

namespace ProEventos.Application.Contratos
{
    public interface IRedeSocialService
    {
        Task<RedeSocialDto[]> SaveByEvento(int eventoId, RedeSocialDto[] models); // Salvar todas as redes sociais do Evento
        Task<bool> DeleteByEvento(int eventoId, int redeSocialId); // Deletar todas as redes sociais do Evento

        Task<RedeSocialDto[]> SaveByPalestrante(int palestranteId, RedeSocialDto[] models); // Salvar todas as redes sociais do Palestrante
        Task<bool> DeleteByPalestrante(int palestranteId, int redeSocialId); // Deletar todas as redes sociais do Palestrante

        Task<RedeSocialDto[]> GetAllByEventoIdAsync(int eventoId); // Pegar todos os Eventos por Id
        Task<RedeSocialDto[]> GetAllByPalestranteIdAsync(int palestranteId); // Pegar todos os Palestrante por Id

        Task<RedeSocialDto> GetRedeSocialEventoByIdsAsync(int eventoId, int RedeSocialId);  // Pegar apenas uma RedesSociais por eventoId
        Task<RedeSocialDto> GetRedeSocialPalestranteByIdsAsync(int PalestranteId, int RedeSocialId); // Pegar apenas uma RedesSociais por palestranteId
    }
}