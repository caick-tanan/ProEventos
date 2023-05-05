using AutoMapper;
using ProEventos.API.Dtos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Domain.Identity;

namespace ProEventos.API.Helpers
{
    public class ProEventosProfile : Profile // Esse Profile vem direto do AutoMapper
    {
        public ProEventosProfile()
        {
            CreateMap<Evento, EventoDto>().ReverseMap(); // O ReverseMap() serve para fazer com que tanto o Evento possa passar para o Dto quando vice e versa 
            CreateMap<Lote, LoteDto>().ReverseMap();  
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();  
            CreateMap<Palestrante, PalestranteDto>().ReverseMap();  
            CreateMap<Palestrante, PalestranteAddDto>().ReverseMap();  
            CreateMap<Palestrante, PalestranteUpdateDto>().ReverseMap();  
            
            CreateMap<User, UserDto>().ReverseMap(); 
            CreateMap<User, UserLoginDto>().ReverseMap(); 
            CreateMap<User, UserUpdateDto>().ReverseMap(); 
        }
    }
}