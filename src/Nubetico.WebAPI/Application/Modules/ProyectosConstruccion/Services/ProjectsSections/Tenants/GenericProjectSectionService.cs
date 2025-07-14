using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectsSections.Interfaces;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectsSections.Tenants
{
    public class GenericProjectSectionService(
        IDbContextFactory<ProyectosConstruccionDbContext> proyectosConstruccciondbContextFactory,
        IDbContextFactory<CoreDbContext> coreConstruccciondbContextFactory) : IProjectSectionService
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _proyectosConstruccciondbContextFactory = proyectosConstruccciondbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreConstruccciondbContextFactory = coreConstruccciondbContextFactory;
        public async Task<ResponseDto<string?>?> ProjectSectionAddAsync(ProjectSectionDataDto request)
        {
            try
            {
                var dbSection = _proyectosConstruccciondbContextFactory.CreateDbContext();
                var dbCore = _coreConstruccciondbContextFactory.CreateDbContext();

                var user = await dbCore.vUsuariosPersonas.FirstOrDefaultAsync(item => item.GuidUsuarioDirectory == request.UserActionGuid!.Value);
                if (user == null)
                    return new(false, "notfounduser");

                await dbSection.AddAsync(new Secciones()
                {
                    //Id_Proyecto = 0,
                    Nombre = request.Name,
                    Descripcion = request.Description,
                    Id_Contratista = request.GeneralContractor!.Value,
                    Fecha_Inicio_Proyectada = request.ProjectedStartDate!.Value,
                    Fecha_Terminacion_Proyectada = request.ProjectedEndDate!.Value,
                    Fecha_Inicio = request.RealStartDate!.Value,
                    Fecha_Terminacion = request.RealEndDate!.Value,
                    Secuencia = request.Sequence,
                    Id_Usuario_Alta = user.IdUsuario,
                    Fecha_Alta = DateTime.Now,
                });

                await dbSection.SaveChangesAsync();
                return new(true, "aqui va el folio ya que lo tenga");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }

        }
    }
}
