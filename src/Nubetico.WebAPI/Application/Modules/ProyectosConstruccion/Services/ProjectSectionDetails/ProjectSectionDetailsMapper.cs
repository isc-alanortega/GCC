using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using System.Collections.Generic;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectSectionDetails
{
    public static class ProjectSectionDetailsMapper
    {
        public static async Task<PaginatedListDto<SectionDetailsDto>> GetSectionsPaginetedListMap(IQueryable<vProyectos_Secciones> query, int offset, int limit)
        {
            int count = await query.CountAsync();
            return  new PaginatedListDto<SectionDetailsDto>
            {
               RecordsTotal = count,
               Data = count > 0 ? await query
                .Skip(offset)
                .Take(limit)
                .Select(item => GetSectionDetailsMap(item)) // Mapeo correcto aquí
                .ToListAsync()
                : []
           };
        }


        public static SectionDetailsDto? GetSectionDetailsMap(vProyectos_Secciones? item) => item == null ? null : new()
        {
            ProjectGuid = item.ProyectoGuid,
            SectionGuid = item.SeccionGuid,
            Project = item.Proyecto,
            Section = item.Seccion,
            Description = item.Descripcion,
            ProjectedStartDate = item.Fecha_Inicio_Proyectada,
            ProjectedEndDate = item.Fecha_Terminacion_Proyectada,
            StartDate = item.Fecha_Inicio,
            EndDate = item.Fecha_Terminacion,
            GeneralConractor = item.Contratista,
            Complete = item.Terminado,
        };

        public static List<SectionLotsDto> GetSectionLotsListMap(List<vSecciones_Lotes> sectionLots) => !sectionLots.Any() ? [] : sectionLots
        .Select(item => new SectionLotsDto()
        {
            ProjectGuid = item.ProyectoGuid,
            SectionGuid = item.SeccionGuid,
            Project = item.Proyecto,
            Subdivision = item.Fraccionamiento,
            SubdivisionStep = item.Fraccionamiento_Etapa,
            Stage = item.Manzana,
            Lots = item.Lote
        }).ToList();
    }
}
