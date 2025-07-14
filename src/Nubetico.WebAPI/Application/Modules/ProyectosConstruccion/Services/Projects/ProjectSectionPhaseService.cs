using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects
{
    public class ProjectSectionPhaseService(
        IDbContextFactory<ProyectosConstruccionDbContext> proeyectosConstruccciondbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        IMapper mapper)
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _proyectosConstrucciondbContextFactory = proeyectosConstruccciondbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;
        private readonly IMapper _mapper = mapper;

        #region PHASES
        public async Task<ResponseDto<ProjectSectionPhaseDto?>?> AddPhaseAsync(ProjectSectionPhaseDto request)
        {
            try
            {
                using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var phase = _mapper.Map<Secciones_Fases>(request);
                    phase.Habilitado = true;

                    await db.Secciones_Fases.AddAsync(phase);
                    await db.SaveChangesAsync();

                    request.PhaseId = phase.Id_Seccion_Fase;

                    return new(success: true, data: request);
                }
            }
            catch (Exception ex)
            {
                return new(success: false, message: ex.Message);
            }
        }

        public async Task<ResponseDto<ProjectSectionPhaseDto?>?> UpdatePhaseAsync(ProjectSectionPhaseDto request)
        {
            try
            {
                using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var phase = await db.Secciones_Fases.FirstAsync(phase => phase.Id_Seccion_Fase == request.PhaseId);

                    _mapper.Map<Secciones_Fases>(request);
                    phase.Habilitado = request.Active!.Value;

                    await db.SaveChangesAsync();

                    return new(success: true, data: request);
                }
            }
            catch (Exception ex)
            {
                return new(success: false, message: ex.Message);
            }
        }

        public async Task<bool> DeletePhasesAsync(int phaseId)
        {
            using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
            {
                var phase = await db.Secciones_Fases.FirstOrDefaultAsync(item => item.Id_Seccion_Fase == phaseId);
                if (phase != null)
                {
                    phase!.Habilitado = false;
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
            }
        }

        public async Task<ResponseDto<List<ProjectSectionPhaseDto>?>?> FindPhasesBySectionsIdAsync(int sectionId, Guid? sectionGuid = null)
        {
            try
            {
                using (var dbProjectSection = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var phases = await dbProjectSection.Secciones_Fases
                        .Where(item => item.Id_Seccion == sectionId && item.Habilitado == true)
                        .OrderBy(item => item.Secuencia)
                        .ToListAsync();

                    var phaseDto = _mapper.Map<List<ProjectSectionPhaseDto>>(phases).ToList();
                    phaseDto.ForEach(item =>
                    {
                        item.SectionGuidTemp = sectionGuid;
                        item.SectionId = sectionId;
                    });

                    if (sectionGuid == null)
                    {
                        sectionGuid = await dbProjectSection.Secciones.Where(section => section.Id_Seccion == sectionId).Select(section => section.UUID).FirstOrDefaultAsync();
                        phaseDto.ForEach(phase => phase.SectionGuidTemp = sectionGuid);
                    }

                    // Pendiente obtener el nombre del proyecto y su id para  posterior uso del detalle 
                    //if(projectId != null) result.ForEach(phase => phase.pr = sectionGuid);

                    return new(success: true, data: phaseDto);
                }
            }
            catch (Exception ex)
            {
                return new(success: false, message: ex.Message);
            }
        }
        #endregion
    }
}
