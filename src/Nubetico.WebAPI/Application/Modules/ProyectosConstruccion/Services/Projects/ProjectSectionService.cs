using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.Sections;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Queries;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects
{
    public class ProjectSectionService(
        IDbContextFactory<ProyectosConstruccionDbContext> proeyectosConstruccciondbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        ProjectSectionPhaseService phasesService,
        IMapper mapper)
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _proyectosConstrucciondbContextFactory = proeyectosConstruccciondbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;
        private readonly ProjectSectionPhaseService _phasesService = phasesService;
        private readonly IMapper _mapper = mapper;

        #region SECTIONS
        public async Task<ResponseDto<ProjectSectionDataDto?>?> AddSectionAsync(ProjectSectionDataDto request)
        {
            try
            {
                using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var section = _mapper.Map<Secciones>(request);
                    section.Id_Usuario_Alta = await GetUserIdByGuid(request.UserActionGuid);
                    section.Fecha_Alta = DateTime.Now;

                    if (request.SectionGuid != null)
                        section.UUID = request.SectionGuid!.Value;

                    await db.Secciones.AddAsync(section);
                    await db.SaveChangesAsync();

                    _mapper.Map(section, request);

                    foreach (var phase in request.ProjectSectionPhase)
                    {
                        phase.SectionId = section.Id_Seccion;
                        var responsePhase = await _phasesService.AddPhaseAsync(phase);

                        if (responsePhase == null || !responsePhase.Success) continue;

                        phase.ProjectId = request.ProjectId;
                        //phase.ProjectName = request.Nombre;
                        phase.PhaseId = responsePhase.Result?.PhaseId;
                    }

                    foreach (var item in request.SectionLots)
                    {
                        item.SectionId = section.Id_Seccion;
                        var result = await AddSectionLotsRelationshipAsync(item);

                        if (result == null && !result!.Success) continue;

                        item.SectionLotsId = result.Result!.SectionLotsId;
                    }

                    return new(success: true, data: request);
                }
            }
            catch (Exception ex)
            {
                return new(success: false, message: ex.Message);
            }
        }

        public async Task<ResponseDto<ProjectSectionDataDto?>?> UpdateSectionAsync(ProjectSectionDataDto request)
        {
            try
            {
                using (var dbProjectSection = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var section = await dbProjectSection.Secciones.FirstOrDefaultAsync(section => section.Id_Seccion == request.SectionId);
                    if (section == null) return new(success: false, message: "no se encontro la sección");


                    _mapper.Map(request, section);
                    section.Habilitado = request.Active;
                    section.Id_Usuario_Modificacion = 1;
                    section.Fecha_Modificacion = DateTime.Now;

                    await dbProjectSection.SaveChangesAsync();

                    var phasesInDb = await _phasesService.FindPhasesBySectionsIdAsync(section.Id_Seccion);
                    if (section.Habilitado == false)
                    {
                        foreach (var phase in phasesInDb!.Result!)
                        {
                            if (request.ProjectSectionPhase.FirstOrDefault(item => item.PhaseId == phase.PhaseId) != null) continue;

                            await DeleteSectionById(phase.PhaseId!.Value);
                        }
                    }
                    else
                    {
                        foreach (var phaseDb in phasesInDb!.Result!)
                        {
                            if (request.ProjectSectionPhase.FirstOrDefault(item => item.PhaseId == phaseDb.PhaseId) != null || phaseDb.PhaseId == null || phaseDb.PhaseId == 0) continue;
                            await _phasesService.DeletePhasesAsync(phaseDb.PhaseId!.Value);
                        }

                        foreach (var phase in request.ProjectSectionPhase)
                        {
                            if (phasesInDb!.Result!.FirstOrDefault(item => item.PhaseId == phase.PhaseId) != null)
                            {
                                await _phasesService.UpdatePhaseAsync(phase);
                                continue;
                            }

                            if (section.Habilitado == true && phase.PhaseId == 0 && phase.Active == true)
                            {
                                var phaseResult = await _phasesService.AddPhaseAsync(phase);
                                if (phaseResult != null && phaseResult.Success && phaseResult.Result != null)
                                {
                                    phase.PhaseId = phaseResult.Result.PhaseId;
                                    phase.ProjectId = section.Id_Proyecto;
                                }
                            }
                        }
                    }

                    return new(success: true, data: request);
                }
            }
            catch (Exception ex)
            {
                return new(success: false, message: "Hubo un error al actualizar los datos");
            }
        }

        public async Task DeleteSectionById(int sectionId)
        {
            using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
            {
                var section = await db.Secciones.FirstOrDefaultAsync(item => item.Id_Seccion == sectionId);
                if (section != null)
                {
                    section.Habilitado = false;
                    await db.SaveChangesAsync();

                    var phasesResult = await _phasesService.FindPhasesBySectionsIdAsync(sectionId);
                    if (phasesResult != null && phasesResult.Success && phasesResult?.Result != null)
                    {
                        foreach (var phase in phasesResult.Result)
                        {
                            await _phasesService.DeletePhasesAsync(phase.PhaseId!.Value);
                        }
                    }
                }
            }
        }

        public async Task<ResponseDto<ProjectSectionFetcherDto>> GetSectionFetchAsync(int subdivisionId, int? sectionId)
        {
            try
            {
                using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var lostList = await GetLotsBySubdivisionIdAsync(subdivisionId, sectionId);
                    return new(success: true, data: new ProjectSectionFetcherDto()
                    {
                        LotsList = lostList.Success ? lostList.Result! : new PaginatedListDto<SectionLotsGridDto>() { Data = [], Error = lostList.Message ?? string.Empty },
                        SectionStatusList = await db.Cat_Estatus.GetCatEstatusDropDown().ToListAsync(),
                        GeneralContractorsList = await db.vContratistas.GetvContratistasElementsDropdown().ToListAsync(),
                        Models = await db.Modelos.GetDropdownModels().ToListAsync(),
                    });
                }
            }
            catch (Exception ex)
            {
                return new(success: true, data: new ProjectSectionFetcherDto()
                {
                    LotsList = new PaginatedListDto<SectionLotsGridDto>() { Data = [], Error = ex.Message },
                    SectionStatusList = [],
                    GeneralContractorsList = [],
                    Models = [],
                });
            }
        }

        public async Task<ResponseDto<IEnumerable<ProjectSectionDataDto>?>> FindSectionsByProjectIdAsync(int projectId)
        {
            try
            {
                using (var dbProjectSection = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var sections = await dbProjectSection.Secciones.
                        Where(item => item.Id_Proyecto == projectId && item.Habilitado == true)
                        .OrderBy(item => item.Secuencia)
                        .ToListAsync();

                    var result = _mapper.Map<List<ProjectSectionDataDto>>(sections);

                    foreach (var section in result)
                    {
                        var phasesResult = await _phasesService.FindPhasesBySectionsIdAsync(section.SectionId!.Value, section.SectionGuid);
                        if (phasesResult == null || !phasesResult.Success || phasesResult.Result == null) continue;

                        section.ProjectSectionPhase = phasesResult.Result;
                    }
                    return new(success: true, data: result);
                }
            }
            catch (Exception ex)
            {
                return new(success: false, message: ex.Message);
            }
        }

        public async Task<IEnumerable<BasicItemSelectDto>> GetProjectSectionSelectDtoAsync(int? contractorId)
        {
            using (var pcDbContext = _proyectosConstrucciondbContextFactory.CreateDbContext())
            {
                List<BasicItemSelectDto> result = await (from seccion in pcDbContext.Secciones
                                                         join proyecto in pcDbContext.Proyectos on seccion.Id_Proyecto equals proyecto.Id_Proyecto
                                                         select new BasicItemSelectDto
                                                         {
                                                             Value = seccion.Id_Seccion,
                                                             Text = $"{proyecto.Nombre} {seccion.Nombre}"
                                                         }).ToListAsync();
                return result;
            }
        }

        #region LOTS
        public async Task<ResponseDto<PaginatedListDto<SectionLotsGridDto>?>> GetLotsBySubdivisionIdAsync(int subdivisionId, int? sectionId)
        {
            try
            {
                using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var lots = await db.Lotes
                            .LotsBySubdivisionId(subdivisionId)
                            .ToListAsync();

                    var lotsDto = _mapper.Map<List<SectionLotsGridDto>>(lots);

                    var result = new PaginatedListDto<SectionLotsGridDto>
                    {
                        RecordsTotal = lotsDto.Count(),
                        Data = lotsDto,
                    };

                    var lotsInSection = await GetLotsInSectionAsync(sectionId);
                    if (lotsInSection.Count > 0 && result?.Data.Count > 0)
                    {
                        foreach (var item in result.Data)
                        {
                            var relationship = lotsInSection.FirstOrDefault(sectionLost => sectionLost.Id_Lote == item.LotId);
                            if (relationship == null) continue;

                            item.SectionLotsId = relationship.Id_Seccion_Lotes;
                            item.IsLotSelected = true;
                        }
                    }

                    return new(success: true, data: result);
                }
            }
            catch (Exception ex)
            {
                return new(success: true, message: "Ocurrrio un problema al recuperar los lotes del fraccionamiento");
            }
        }

        public async Task<List<Secciones_Lotes>> GetLotsInSectionAsync(int? sectionId)
        {
            using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
            {
                var lots = await db.Secciones_Lotes.Where(item => item.Id_Seccion == sectionId).ToListAsync();
                return lots;
            }
        }
        #endregion

        public async Task<ResponseDto<SectionLotsGridDto>> AddSectionLotsRelationshipAsync(SectionLotsGridDto sectionLotsDto)
        {
            using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
            {
                var sectionLots = _mapper.Map<Secciones_Lotes>(sectionLotsDto);

                await db.Secciones_Lotes.AddAsync(sectionLots);
                await db.SaveChangesAsync();

                sectionLotsDto.SectionLotsId = sectionLots.Id_Seccion_Lotes;
            }

            return new(success: true, data: sectionLotsDto);
        }

        public async Task DeleteSectionLotsRelationshipAsync(int sectionLostId)
        {
            try
            {
                using (var db = _proyectosConstrucciondbContextFactory.CreateDbContext())
                {
                    var sectionLost = await db.Secciones_Lotes.FirstOrDefaultAsync(item => item.Id_Seccion_Lotes == sectionLostId);
                    if (sectionLost == null) return;

                    //sectionLost.Habilitado = false;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region UTILS
        private async Task<int> GetUserIdByGuid(Guid? userGuid)
        {
            var dbCore = _coreDbContextFactory.CreateDbContext();
            var user = await dbCore.vUsuariosPersonas.FirstAsync(item => item.GuidUsuarioDirectory == userGuid!.Value);
            return user.IdUsuario;
        }
        #endregion
    }
}
