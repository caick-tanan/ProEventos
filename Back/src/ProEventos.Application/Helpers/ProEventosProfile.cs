using AutoMapper;
using ProEventos.API.Dtos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;

namespace ProEventos.API.Helpers
{
    public class ProEventosProfile : Profile // Esse Profile vem direto do AutoMapper
    {
        public ProEventosProfile()
        {
            CreateMap<Evento, EventoDto>().ReverseMap(); // O ReverseMap() serve para fazer com que tanto o Evento possa passar para o Dto quando vice e versa 
            CreateMap<Lote, LoteDto>().ReverseMap();  
            CreateMap<Palestrante, PalestranteDto>().ReverseMap();  
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();  
        }
    }
}