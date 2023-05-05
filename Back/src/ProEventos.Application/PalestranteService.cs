using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.API.Dtos;
using AutoMapper;
using ProEventos.Persistence.Models;
using ProEventos.Application.Dtos;

namespace ProEventos.Application
{
    public class PalestranteService : IPalestranteService
    {
        private readonly IPalestrantePersist _palestrantePersist;
        private readonly IMapper _mapper;

        public PalestranteService(IPalestrantePersist palestrantePersist,
                                  IMapper mapper)
        {
            _palestrantePersist = palestrantePersist;
            _mapper = mapper;
        }

         public async Task<PalestranteDto> AddPalestrantes(int userId, PalestranteAddDto model)
        {
            try
            {
                var Palestrante = _mapper.Map<Palestrante>(model);
                Palestrante.UserId = userId;

                _palestrantePersist.Add<Palestrante>(Palestrante);

                if (await _palestrantePersist.SaveChangesAsync())
                {
                    var PalestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);

                    return _mapper.Map<PalestranteDto>(PalestranteRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PalestranteDto> UpdatePalestrante(int userId, PalestranteUpdateDto model)
        {
            try
            {
                var Palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);
                if (Palestrante == null) return null;

                model.Id = Palestrante.Id;
                model.UserId = userId;

                _mapper.Map(model, Palestrante);

                _palestrantePersist.Update<Palestrante>(Palestrante);

                if (await _palestrantePersist.SaveChangesAsync())
                {
                    var PalestranteRetorno = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, false);

                    return _mapper.Map<PalestranteDto>(PalestranteRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<PalestranteDto>> GetAllPalestrantesAsync(PageParams pageParams ,bool includeEventos = false)
        {
            try
            {
                var Palestrantes = await _palestrantePersist.GetAllPalestrantesAsync(pageParams, includeEventos);
                if (Palestrantes == null) return null;

                var resultado = _mapper.Map<PageList<PalestranteDto>>(Palestrantes);

                resultado.CurrentPage = Palestrantes.CurrentPage;
                resultado.TotalPages = Palestrantes.TotalPages;
                resultado.PageSize = Palestrantes.PageSize;
                resultado.TotalCount = Palestrantes.TotalCount;

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PalestranteDto> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
        {
            try
            {
                var Palestrante = await _palestrantePersist.GetPalestranteByUserIdAsync(userId, includeEventos);
                if (Palestrante == null) return null;

                var resultado = _mapper.Map<PalestranteDto>(Palestrante); // Dado o meu objeto DTO eu vou mapear meu evento com os campos que eu queira

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