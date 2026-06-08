using AutoMapper;
using ServidorApi.DTOs;
using ServidorApi.Models;
 
namespace ServidorApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ── USUÁRIO ───────────────────────────────────────────────────────
            CreateMap<Usuario, UsuarioResponseDTO>()
                .ForMember(d => d.Email,
                    o => o.MapFrom(u => u.Conta != null ? u.Conta.Email : string.Empty))
                .ForMember(d => d.DataCriacao,
                    o => o.MapFrom(u => u.Conta != null ? u.Conta.DataCriacao : DateTime.Now));
 
            CreateMap<UsuarioCreateDTO, Usuario>()
                .ForMember(d => d.ID,    o => o.Ignore())
                .ForMember(d => d.Conta, o => o.Ignore());
 
            CreateMap<UsuarioUpDateDTO, Usuario>();
 
            // ── CATEGORIA ─────────────────────────────────────────────────────
            CreateMap<Categoria, CategoriaResponseDTO>().ReverseMap();
 
            // ── ENTRADA ───────────────────────────────────────────────────────
            // IDConta removido — mapeamento agora usa IDUsuario
            CreateMap<Entradas, EntradasResponseDTO>().ReverseMap();
            CreateMap<EntradasUpdateDTO, Entradas>();
 
            // ── SAÍDA ─────────────────────────────────────────────────────────
            // IDConta removido — mapeamento agora usa IDUsuario
            CreateMap<Saida, SaidaResponseDTO>().ReverseMap();
            CreateMap<SaidaUpdateDTO, Saida>();
        }
    }
}