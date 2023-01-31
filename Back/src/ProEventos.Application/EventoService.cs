using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.API.Dtos;
using AutoMapper;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersist _geralPersist;
        private readonly IEventoPersist _eventoPersist;
        private readonly IMapper _mapper;

        public EventoService(IGeralPersist geralPersist,
                            IEventoPersist eventoPersist,
                            IMapper mapper)
        {
            _eventoPersist = eventoPersist;
            _geralPersist = geralPersist;
            _mapper = mapper;

        }

        public async Task<EventoDto> AddEventos(EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model); //mapeando o model para o evento a partir do EventoDto

                _geralPersist.Add<Evento>(evento); // Adicionando um evento ao Model com as imformações do EventoDto
                if (await _geralPersist.SaveChangesAsync())
                {  // Se salvar e retornar true 
                    var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(evento.Id, false); // Indo de Evento para Dto
                    return _mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<EventoDto> UpdateEvento(int eventoId, EventoDto model)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, false); // Vai pegar o evento.Id
                if (evento == null) return null; // caso seja nulo vai retornar nulo

                model.Id = evento.Id; // se não for nulo, pega o evento
                
                _mapper.Map(model, evento);

                _geralPersist.Update<Evento>(evento); 
                
                if (await _geralPersist.SaveChangesAsync()) //caso salve
                {
                    var eventoRetorno = await _eventoPersist.GetEventoByIdAsync(evento.Id, false); // Indo de Evento para Dto
                    return _mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEvento(int eventoId)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, false); // vai pegar o evento.id para ser deletado
                if (evento == null) throw new Exception("Evento para delete não encontrado."); // caso seja nulo lançará a execeção

                _geralPersist.Delete<Evento>(evento); // caso encontre o id para ser deletado fará a deleção
                return await _geralPersist.SaveChangesAsync(); // caso delete irá salvar a alteração
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // caso não salve lançará a exceção
            }
        }
        public async Task<EventoDto[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosAsync(includePalestrantes);
                if (eventos == null) return null;

                var resultado = _mapper.Map<EventoDto[]>(eventos);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<EventoDto[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await _eventoPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
                if (eventos == null) return null;

                var resultado = _mapper.Map<EventoDto[]>(eventos);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDto> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await _eventoPersist.GetEventoByIdAsync(eventoId, includePalestrantes);
                if (evento == null) return null;

                var resultado = _mapper.Map<EventoDto>(evento); // Dado o meu objeto DTO eu vou mapear meu evento com os campos que eu queira

                // var eventoRetorno = new EventoDto() //DTOS -> vai retornar apenas a lista abaixo com os campos que eu queira sem expor a API com todos os outros dados caso tenha
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