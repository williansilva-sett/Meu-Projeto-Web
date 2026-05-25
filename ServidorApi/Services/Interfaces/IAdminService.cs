using ServidorApi.DTOs;

namespace ServidorApi.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDTO> ObterDashboardAsync();
    }
}