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
    public class RedeSocialService : IRedeSocialService
    {
        private readonly IRedeSocialPersist _redeSocialPersist;
        private readonly IMapper _mapper;

        public RedeSocialService(IRedeSocialPersist redeSocialPersist,
                            IMapper mapper)
        {
            _redeSocialPersist = redeSocialPersist;
            _mapper = mapper;

        }

        public async Task AddRedeSocial(int Id, RedeSocialDto model, bool isEvento)
        {
            try
            {
                var RedeSocial = _mapper.Map<RedeSocial>(model); //mapeando o model para o RedeSocial a partir do RedeSocialDto
                if(isEvento){ // Se for evento vai adicionar a rede social ao evento
                    RedeSocial.EventoId = Id;
                    RedeSocial.PalestranteId = null;
                }else{  // Se for palestrante vai adicionar a rede social ao palestrante
                    RedeSocial.EventoId = null;
                    RedeSocial.PalestranteId = Id;
                }
                
                _redeSocialPersist.Add<RedeSocial>(RedeSocial); // Adicionando um lote ao Model com as imformações do LoteDto
                
                await _redeSocialPersist.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<RedeSocialDto[]> SaveByEvento(int eventoId, RedeSocialDto[] models)
        {
            try
            {
                var RedeSocials = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId); // Vai pegar todas as RedeSocial daquele evento

                if (RedeSocials == null) return null; // caso seja nulo vai retornar nulo

                foreach (var model in models) // pego cada model subo na listagem de RedeSocials que possuem apenas os RedeSocials do meu evento 
                {
                    if (model.Id == 0)
                    {
                        await AddRedeSocial(eventoId, model, true);
                    }
                    else
                    {
                        var RedeSocial = RedeSocials.FirstOrDefault(RedeSocial => RedeSocial.Id == model.Id); // Pego o 1º ou or Default

                        model.EventoId = eventoId; // atribuo

                        _mapper.Map(model, RedeSocial);

                        _redeSocialPersist.Update<RedeSocial>(RedeSocial); // Atualizo

                        await _redeSocialPersist.SaveChangesAsync(); //caso salve
                    }
                }

                var RedeSocialRetorno = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId); 
                
                return _mapper.Map<RedeSocialDto[]>(RedeSocialRetorno);
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


         public async Task<RedeSocialDto[]> SaveByPalestrante(int palestranteId, RedeSocialDto[] models)
        {
            try
            {
                var RedeSocials = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId); // Vai pegar todas as RedeSocial daquele Palestrante

                if (RedeSocials == null) return null; // caso seja nulo vai retornar nulo

                foreach (var model in models) // pego cada model subo na listagem de RedeSocials que possuem apenas os RedeSocials do meu Palestrante 
                {
                    if (model.Id == 0)
                    {
                        await AddRedeSocial(palestranteId, model, false);
                    }
                    else
                    {
                        var RedeSocial = RedeSocials.FirstOrDefault(RedeSocial => RedeSocial.Id == model.Id); // Pego o 1º ou or Default

                        model.PalestranteId = palestranteId; // atribuo

                        _mapper.Map(model, RedeSocial);

                        _redeSocialPersist.Update<RedeSocial>(RedeSocial); // Atualizo

                        await _redeSocialPersist.SaveChangesAsync(); //caso salve
                    }
                }

                var RedeSocialRetorno = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId); 
                
                return _mapper.Map<RedeSocialDto[]>(RedeSocialRetorno);
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> DeleteByEvento(int eventoId, int redeSocialId)
        {
            try
            {
                var RedeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId); // vai pegar o evento.id para ser deletado

                if (RedeSocial == null) throw new Exception("Rede Social por Evento para delete não encontrado."); // caso seja nulo lançará a execeção

                _redeSocialPersist.Delete<RedeSocial>(RedeSocial); // caso encontre o id para ser deletado fará a deleção

                return await _redeSocialPersist.SaveChangesAsync(); // caso delete irá salvar a alteração
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // caso não salve lançará a exceção
            }
        }

        public async Task<bool> DeleteByPalestrante(int palestranteId, int redeSocialId)
        {
            try
            {
                var RedeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, redeSocialId); // vai pegar o Palestrante.id para ser deletado

                if (RedeSocial == null) throw new Exception("Rede Social por Palestrante para delete não encontrado."); // caso seja nulo lançará a execeção

                _redeSocialPersist.Delete<RedeSocial>(RedeSocial); // caso encontre o id para ser deletado fará a deleção

                return await _redeSocialPersist.SaveChangesAsync(); // caso delete irá salvar a alteração
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // caso não salve lançará a exceção
            }
        }

        public async Task<RedeSocialDto[]> GetAllByEventoIdAsync(int eventoId)
        {
            try
            {
                var RedeSocials = await _redeSocialPersist.GetAllByEventoIdAsync(eventoId);
                if (RedeSocials == null) return null;

                var resultado = _mapper.Map<RedeSocialDto[]>(RedeSocials);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

         public async Task<RedeSocialDto[]> GetAllByPalestranteIdAsync(int palestranteId)
        {
            try
            {
                var RedeSocials = await _redeSocialPersist.GetAllByPalestranteIdAsync(palestranteId);
                if (RedeSocials == null) return null;

                var resultado = _mapper.Map<RedeSocialDto[]>(RedeSocials);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RedeSocialDto> GetRedeSocialEventoByIdsAsync(int eventoId, int RedeSocialId)
        {
            try
            {
                var RedeSocial = await _redeSocialPersist.GetRedeSocialEventoByIdsAsync(eventoId, RedeSocialId);
                if (RedeSocial == null) return null;

                var resultado = _mapper.Map<RedeSocialDto>(RedeSocial); // Dado o meu objeto DTO eu vou mapear meu evento com os campos que eu queira

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

          public async Task<RedeSocialDto> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int RedeSocialId)
        {
            try
            {
                var RedeSocial = await _redeSocialPersist.GetRedeSocialPalestranteByIdsAsync(palestranteId, RedeSocialId);
                if (RedeSocial == null) return null;

                var resultado = _mapper.Map<RedeSocialDto>(RedeSocial); // Dado o meu objeto DTO eu vou mapear meu evento com os campos que eu queira

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