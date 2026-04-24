using AutoMapper;
using ServidorApi.Models;
using ServidorApi.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Define que Usuario pode virar UsuarioResponseDTO e vice-versa
        CreateMap<Usuario, UsuarioResponseDTO>();
    }
}