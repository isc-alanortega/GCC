using Azure;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectsSections.Interfaces
{
    public interface IProjectSectionService
    {
        public Task<ResponseDto<string?>> ProjectSectionAddAsync(ProjectSectionDataDto request);
    }
}
