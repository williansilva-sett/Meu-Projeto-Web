using AutoMapper;
using ServidorApi.DTOs;
using ServidorApi.Models;

namespace ServidorApi.Mappings // Verifique se o seu namespace é este
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---USUÁRIO---
            CreateMap<Usuario, UsuarioResponseDTO>();
            CreateMap<UsuarioCreateDTO, Usuario>()
                .ForMember(d => d.Senha, o => o.Ignore())
                .ForMember(d => d.ID, o => o.Ignore())
                .ForMember(d => d.DataCriacao, o => o.Ignore());
            CreateMap<UsuarioUpDateDTO, Usuario>()
                .ForMember(d => d.Senha, o => o.Ignore());

            // ---CONTA---
            CreateMap<Conta, ContaResponseDTO>().ReverseMap();
            CreateMap<ContaUpdateDTO, Conta>();
            // ---CATEGORIA---
            CreateMap<Categoria, CategoriaResponseDTO>().ReverseMap();  
            // ---ENTRADA---
            CreateMap<Entradas, EntradasResponseDTO>().ReverseMap();
            CreateMap<EntradasUpdateDTO, Entradas>();
            // ---SAÌDA---
            CreateMap<Saida, SaidaResponseDTO>().ReverseMap();
            CreateMap<SaidaUpdateDTO, Saida>();
        }       
    }
}