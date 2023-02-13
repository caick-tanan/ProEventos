using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.API.Dtos;
using AutoMapper;
using ProEventos.Application.Dtos;
using System.Linq;

namespace ProEventos.Application
{
    public class LoteService : ILoteService
    {
        private readonly IGeralPersist _geralPersist;
        private readonly ILotePersist _lotePersist;
        private readonly IMapper _mapper;

        public LoteService(IGeralPersist geralPersist,
                            ILotePersist lotePersist,
                            IMapper mapper)
        {
            _lotePersist = lotePersist;
            _geralPersist = geralPersist;
            _mapper = mapper;

        }

        public async Task AddLote(int eventoId, LoteDto model)
        {
            try
            {
                var lote = _mapper.Map<Lote>(model); //mapeando o model para o lote a partir do LoteDto

                lote.EventoId = eventoId;

                _geralPersist.Add<Lote>(lote); // Adicionando um lote ao Model com as imformações do LoteDto
                
                await _geralPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<LoteDto[]> SaveLotes(int eventoId, LoteDto[] models)
        {
            try
            {
                var lotes = await _lotePersist.GetLotesByEventoIdAsync(eventoId); // Vai pegar o Lote.Id

                if (lotes == null) return null; // caso seja nulo vai retornar nulo

                foreach (var model in models) // pego cada model subo na listagem de lotes que possuem apenas os lotes do meu evento 
                {
                    if (model.Id == 0)
                    {
                        await AddLote(eventoId, model);
                    }
                    else
                    {
                        var lote = lotes.FirstOrDefault(lote => lote.Id == model.Id);

                        model.EventoId = eventoId; // se não for nulo, pega o evento

                        _mapper.Map(model, lote);

                        _geralPersist.Update<Lote>(lote);

                        await _geralPersist.SaveChangesAsync(); //caso salve
                    }
                }

                var loteRetorno = await _lotePersist.GetLotesByEventoIdAsync(eventoId); // Indo de Evento para Dto
                
                return _mapper.Map<LoteDto[]>(loteRetorno);
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteLote(int eventoId, int loteId)
        {
            try
            {
                var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId); // vai pegar o evento.id para ser deletado

                if (lote == null) throw new Exception("Lote para delete não encontrado."); // caso seja nulo lançará a execeção

                _geralPersist.Delete<Lote>(lote); // caso encontre o id para ser deletado fará a deleção

                return await _geralPersist.SaveChangesAsync(); // caso delete irá salvar a alteração
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // caso não salve lançará a exceção
            }
        }

        public async Task<LoteDto[]> GetLotesByEventoIdAsync(int eventoId)
        {
            try
            {
                var lotes = await _lotePersist.GetLotesByEventoIdAsync(eventoId);
                if (lotes == null) return null;

                var resultado = _mapper.Map<LoteDto[]>(lotes);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoteDto> GetLoteByIdsAsync(int eventoId, int loteId)
        {
            try
            {
                var lote = await _lotePersist.GetLoteByIdsAsync(eventoId, loteId);
                if (lote == null) return null;

                var resultado = _mapper.Map<LoteDto>(lote); // Dado o meu objeto DTO eu vou mapear meu evento com os campos que eu queira

                // var eventoRetorno = new LoteDto() //DTOS -> vai retornar apenas a lista abaixo com os campos que eu queira sem expor a API com todos os outros dados caso tenha
                // {
                //     Id = evento.Id,
                //     Local = evento.Local,
                //     DataEvento = evento.DataEvento.ToString(),
                //     Tema = evento.Tema,
                //     QtdPessoas = evento.QtdPessoas,
                //     ImageURL = evento.ImageURL,
                //     Telefone = evento.Telefone,
                //     Email = evento.Email
                // };

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}